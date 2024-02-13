using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static CubeMeshData;
using Object = UnityEngine.Object;

public class BlockAtlasTexturePickerEditorWindow : EditorWindow
{
    [SerializeField] private int m_SelectedIndex = -1;
    [SerializeField] private Texture2D m_atlasTexture;
    private VisualElement _inspectorPanel;
    private static EditorWindow _wnd;
    private ScrollView _atlasTextureView;
    private ObjectField _atlasObjectField;
    private TextureUvSelector _textureUvSelector;
    private ListView _blockDataList;

    [MenuItem("Tools/ " + nameof(BlockAtlasTexturePickerEditorWindow))]
    public static void ShowMyEditor()
    {
        // This method is called when the user selects the menu item in the Editor
        _wnd = GetWindow<BlockAtlasTexturePickerEditorWindow>();
        _wnd.titleContent = new GUIContent(nameof(BlockAtlasTexturePickerEditorWindow));

        // Limit size of the window
        _wnd.minSize = new Vector2(960, 540);
        _wnd.maxSize = new Vector2(1920, 1080);
    }

    public void CreateGUI()
    {
        // Get a list of all BlockData in the project
        var allObjectGuids = AssetDatabase.FindAssets($"t:{nameof(BlockData)}");

        var allObjects = new List<BlockData>();
        foreach (var guid in allObjectGuids)
            allObjects.Add(AssetDatabase.LoadAssetAtPath<BlockData>(AssetDatabase.GUIDToAssetPath(guid)));

        var splitViewVertical = new TwoPaneSplitView(0, 540, TwoPaneSplitViewOrientation.Vertical);

        var splitViewHorizontal = new TwoPaneSplitView(1, 960, TwoPaneSplitViewOrientation.Horizontal);

        splitViewVertical.Add(splitViewHorizontal);
        _atlasTextureView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        splitViewVertical.Add(_atlasTextureView);

        PopulateAtlasObjectField(rootVisualElement);

        // Add the panel to the visual tree by adding it as a child to the root element
        rootVisualElement.Add(splitViewVertical);

        PopulateBlockDataListView(splitViewHorizontal, allObjects);
        PopulateInspectorPanel(splitViewHorizontal);

        RegisterBlockDataListEvents();
    }

    private void PopulateAtlasObjectField(VisualElement parent)
    {
        _atlasObjectField = new ObjectField("Atlas Texture")
        {
            allowSceneObjects = false,
            objectType = typeof(Texture2D),
        };

        _atlasObjectField.RegisterCallback<ChangeEvent<Object>>(OnAtlasTextureChanged);

        if (m_atlasTexture != null)
        {
            _atlasObjectField.value = m_atlasTexture;
            UpdateAtlasTexture();
        }

        parent.Add(_atlasObjectField);
    }

    private void PopulateBlockDataListView(VisualElement parent, List<BlockData> allObjects)
    {
        _blockDataList = new ListView();

        // Initialize the list view with all sprites' names
        _blockDataList.makeItem = () => new Label();
        _blockDataList.bindItem = (item, index) => { (item as Label).text = allObjects[index].name; };
        _blockDataList.itemsSource = allObjects;

        parent.Add(_blockDataList);
    }

    private void RegisterBlockDataListEvents()
    {
        // React to the user's selection
        _blockDataList.selectionChanged += OnBlockDataSelectionChange;
        // Restore the selection index from before the hot reload 
        _blockDataList.selectedIndex = m_SelectedIndex;
        // Store the selection index when the selection changes
        _blockDataList.selectionChanged += (items) => { m_SelectedIndex = _blockDataList.selectedIndex; };
    }

    private void PopulateInspectorPanel(VisualElement parent)
    {
        _inspectorPanel = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
        parent.Add(_inspectorPanel);
    }

    private void OnAtlasTextureChanged(ChangeEvent<Object> evt)
    {
        if (evt.newValue == null)
            return;
        m_atlasTexture = _atlasObjectField.value as Texture2D;
        Debug.Log($"Changed Atlas : {evt.newValue.name}");

        UpdateAtlasTexture();
    }

    private void UpdateAtlasTexture()
    {
        if (_textureUvSelector != null)
        {
            _textureUvSelector.Clear();
            _textureUvSelector.UnregisterCallback<PointerDownEvent>(OnAtlasTexturePointerDown);
        }

        _atlasTextureView.Clear();

        _textureUvSelector = new(m_atlasTexture);
        _textureUvSelector.RegisterCallback<PointerDownEvent>(OnAtlasTexturePointerDown);
        _atlasTextureView.Add(_textureUvSelector);
    }

    private void OnAtlasTexturePointerDown(PointerDownEvent evt)
    {
        Debug.Log($"Atlas Texture Pointer Down: target {evt.currentTarget} | \n delta Pos {evt.deltaPosition} | \n localPos {evt.localPosition} | \n originalMousePos {evt.originalMousePosition} | \n pos {evt.position}");

        var selectedBlockData = _blockDataList.selectedItem as BlockData;

        if (selectedBlockData == null)
            return;

        Debug.Log($"x: {evt.localPosition.x} y: {evt.localPosition.y} !y: {_textureUvSelector.image.width - evt.localPosition.y}");

        var coordinate = _textureUvSelector.CalculateImageCoordinates(evt.localPosition);
        var maxRows = _textureUvSelector.GetMaxRows() - 1;

        Debug.Log($"Column: {coordinate.x} Row: {maxRows - coordinate.y}");

        bool success = _textureUvSelector.TryAddCoordinateBox(coordinate, Color.green);

        Debug.Log($"{(success ? "Success" : "Failed")} to create Coordinate Box at: {coordinate}");
    }

    private void OnBlockDataSelectionChange(IEnumerable<object> selectedItems)
    {
        UpdateInspectorPanel();
    }

    private void UpdateInspectorPanel()
    {
        // Clear all previous content from the pane
        if (_inspectorPanel == null)
            return;

        _inspectorPanel.Clear();

        // Get the selected sprite
        var selected = _blockDataList.selectedItem as BlockData;
        if (selected == null)
            return;

        // Add a new Image control and display the sprite
        var inspectorElement = new InspectorElement(selected);
        var textureDataUI = inspectorElement.Q<CubeTextureDataUI>();
        textureDataUI.OnFaceButtonPressed += OnFaceButtonPressed;

        _inspectorPanel.Add(inspectorElement);

        var buttonShowSelectedUVs = new Button(ShowSelectedUVs)
        {
            text = "Show Selected UVs"
        };
        _inspectorPanel.Add(buttonShowSelectedUVs);

    }

    private void OnFaceButtonPressed(object sender, FaceArgs e)
    {
        Debug.Log($"Pressed {e.faceOrientation}");
        var localFaceOrientation = e.faceOrientation;

        _textureUvSelector.GetCoordinatesOnMouseClick((coordinate) =>
        {
            if (TryGetSelectedBlockData(out var blockData))
            {
                var maxRows = _textureUvSelector.GetMaxRows() - 1;
                var uv = new Vector2Int(coordinate.x, maxRows - coordinate.y);
                Debug.Log($"{localFaceOrientation} Selected Coordinate:{coordinate} | UV: {uv}");

                switch (localFaceOrientation)
                {
                    case FaceOrientation.Front:
                        blockData.textureData.frontUV = uv;
                        break;
                    case FaceOrientation.Top:
                        blockData.textureData.topUV = uv;
                        break;
                    case FaceOrientation.Back:
                        blockData.textureData.backUV = uv;
                        break;
                    case FaceOrientation.Bottom:
                        blockData.textureData.bottomUV = uv;
                        break;
                    case FaceOrientation.Right:
                        blockData.textureData.rightUV = uv;
                        break;
                    case FaceOrientation.Left:
                        blockData.textureData.leftUV = uv;
                        break;
                    default:
                        break;
                }

                EditorUtility.SetDirty(blockData);
            }

        });

    }

    private bool TryGetSelectedBlockData(out BlockData blockData)
    {
        blockData = null;

        if (_blockDataList.selectedItem == null)
            return false;

        blockData = _blockDataList.selectedItem as BlockData;
        return true;
    }

    private void ShowSelectedUVs()
    {
        if (_textureUvSelector == null)
        {
            Debug.LogError("No Atlas Texture Selected");
            return;
        }

        if (TryGetSelectedBlockData(out var selectedBlockData))
        {
            _textureUvSelector.ClearAllCoordinateBoxes();

            foreach (var uv in selectedBlockData.textureData.faceUVs)
            {
                var maxRows = _textureUvSelector.GetMaxRows() - 1;
                var coordinate = new Vector2Int(uv.Value.x, maxRows - uv.Value.y);

                bool success = _textureUvSelector.TryAddCoordinateBox(coordinate, Color.blue);

                Debug.Log($"{(success ? "Success" : "Failed")} to create Coordinate Box at: {uv.Value}");
            }
        }
    }

    private void OnDestroy()
    {
        if (_textureUvSelector != null)
        {
            _textureUvSelector.Clear();
            _textureUvSelector.UnregisterCallback<PointerDownEvent>(OnAtlasTexturePointerDown);
        }
    }

}
