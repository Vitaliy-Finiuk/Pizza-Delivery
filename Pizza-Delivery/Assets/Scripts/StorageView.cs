using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class StorageView : MonoBehaviour
{
	private ResourceUnit[] _resourceUnits;
	private bool[] _resourseUnitsPosedFlags;
	private static UnityException _dimensionsAreSameException;

	[SerializeField] private int _rows;
	[SerializeField] private int _columns;
	[SerializeField] private int _floors;
	[SerializeField] private float _cellSize;
	[SerializeField] private Vector3 _cellsOffset;
	[SerializeField] private Transform _firstCell;
	[SerializeField] private Dimension _firstPriorityDimension;
	[SerializeField] private Dimension _secondPriorityDimension;
	[SerializeField] private float _positioningSpeed;
	[SerializeField] private float _scalingSpeed;
	[SerializeField] private float _rotationSpeed;
	[SerializeField] private float _maxDistanceForStopPositioning;

	[Header("Debug")]
	public bool drawCells;

	public int CellsCount => _rows * _columns * _floors;

	public int BusyCellsCount => _resourceUnits.Count(x => x != null);

	static StorageView()
	{
		_dimensionsAreSameException = new UnityException("Dimensions are same");
	}

	private void Awake() => Init();

	private void Update()
	{
		for (int i = 0; i < _resourceUnits.Length; i++)
		{
			if (_resourceUnits[i] == null || _resourseUnitsPosedFlags[i] == true)
				continue;

			Transform resourceUnit = _resourceUnits[i].transform;
			Vector3 cellPosition = CalculatePosition(i);
			resourceUnit.position = Vector3.Lerp(resourceUnit.position, cellPosition,
													_positioningSpeed * Time.deltaTime);
			resourceUnit.localScale = Vector3.Lerp(resourceUnit.localScale, Vector3.one * _cellSize,
													_scalingSpeed * Time.deltaTime);
			resourceUnit.rotation = Quaternion.Lerp(resourceUnit.rotation, _firstCell.rotation,
													_rotationSpeed * Time.deltaTime);

			if (Vector3.Distance(resourceUnit.position, cellPosition) <= _maxDistanceForStopPositioning)
			{
				resourceUnit.position = cellPosition;
				resourceUnit.rotation = _firstCell.rotation;
				resourceUnit.localScale = Vector3.one * _cellSize;
				_resourseUnitsPosedFlags[i] = true;
			}
		}
	}

	private void Init()
	{
		_resourceUnits = new ResourceUnit[CellsCount];
		_resourseUnitsPosedFlags = new bool[CellsCount];

		for (int i = 0; i < BusyCellsCount; i++)
			_resourseUnitsPosedFlags[i] = false;
	}

	public void OnPutIn(object sender, ResourceUnit resourceUnit)
	{
		resourceUnit.transform.parent = this.transform;
		int freeCellIndex = BusyCellsCount;
		_resourceUnits[freeCellIndex] = resourceUnit;
		_resourseUnitsPosedFlags[freeCellIndex] = false;
	}

	public void OnPullOut(object sender, ResourceUnit resourceUnit)
	{
		if (resourceUnit.transform.parent == this.transform)
			resourceUnit.transform.parent = null;

		int cellForRecord = 0;

		for (int i = 0; i < _resourceUnits.Length; i++)
		{
			_resourceUnits[cellForRecord] = _resourceUnits[i];
			_resourseUnitsPosedFlags[cellForRecord] = false;

			if (_resourceUnits[i] != resourceUnit && _resourceUnits[i] != null)
				cellForRecord++;
		}

		for (int i = cellForRecord; i < _resourceUnits.Length; i++)
			_resourceUnits[i] = null;
	}

	private Vector3 CalculatePosition(int cellIndex)
	{
		int column, row, floor;

		SetDimensions(out column, out row, out floor, cellIndex);

		Vector3 pos = _firstCell.position;
		pos += _firstCell.right * _cellsOffset.x * column;
		pos += _firstCell.forward * _cellsOffset.z * row;
		pos += _firstCell.up * _cellsOffset.y * floor;

		return pos;
	}

	private void SetDimensions(out int column, out int row, out int floor, int i)
	{
		if (_firstPriorityDimension == Dimension.Column && _secondPriorityDimension == Dimension.Row)
			SetDimensions(out column, out row, out floor, _columns, _rows, i);
		else if (_firstPriorityDimension == Dimension.Column && _secondPriorityDimension == Dimension.Floor)
			SetDimensions(out column, out floor, out row, _columns, _floors, i);
		else if (_firstPriorityDimension == Dimension.Row && _secondPriorityDimension == Dimension.Column)
			SetDimensions(out row, out column, out floor, _rows, _columns, i);
		else if (_firstPriorityDimension == Dimension.Row && _secondPriorityDimension == Dimension.Floor)
			SetDimensions(out row, out floor, out column, _rows, _floors, i);
		else if (_firstPriorityDimension == Dimension.Floor && _secondPriorityDimension == Dimension.Column)
			SetDimensions(out floor, out column, out row, _floors, _columns, i);
		else if (_firstPriorityDimension == Dimension.Floor && _secondPriorityDimension == Dimension.Row)
			SetDimensions(out floor, out row, out column, _floors, _rows, i);
		else
			throw _dimensionsAreSameException;
	}

	private void SetDimensions(out int x, out int y, out int z, int maxX, int maxY, int index)
	{
		x = (index % (maxX * maxY)) % maxX;
		y = (index % (maxX * maxY)) / maxX;
		z = index / (maxX * maxY);
	}

	private void OnDrawGizmosSelected()
	{
		if (drawCells == false || _firstCell == null)
			return;

		for (int i = 0; i < CellsCount; i++)
		{
			Gizmos.DrawSphere(CalculatePosition(i), _cellSize / 2);
		}
	}
}
