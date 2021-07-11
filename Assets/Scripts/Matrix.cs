using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Matrix : MonoBehaviour
{
    #region --Fields / Properties--

    /// <summary>
    /// Prefab for the rows of each matrix.
    /// </summary>
    [Header("UI")]
    [SerializeField]
    private GameObject _rowPrefab;

    /// <summary>
    /// Prefab for the columns of each matrix.
    /// </summary>
    [SerializeField]
    private GameObject _columnPrefab;

    /// <summary>
    /// Transform component of the first matrix (A).
    /// </summary>
    [SerializeField]
    private Transform _matrixATransform;

    /// <summary>
    /// Transform component of the second matrix (B).
    /// </summary>
    [SerializeField]
    private Transform _matrixBTransform;

    /// <summary>
    /// Transform component of the product matrix.
    /// </summary>
    [SerializeField]
    private Transform _matrixProductTransform;

    /// <summary>
    /// Text field for the first matrix's (A) row count.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _matrixARows;

    /// <summary>
    /// Text field for the first matrix's (A) column count.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _matrixAColumns;

    /// <summary>
    /// Text field for the second matrix's (B) row count.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _matrixBRows;

    /// <summary>
    /// Text field for the second matrix's (B) column count.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _matrixBColumns;

    /// <summary>
    /// Text field for the product matrix's row count.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _productMatrixRows;

    /// <summary>
    /// Text field for the product matrix's column count.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _productMatrixColumns;
    
    /// <summary>
    /// Calculates a random integer between -_range and _range.
    /// </summary>
    [Header("Matrices")]
    [SerializeField]
    private int _range;

    /// <summary>
    /// The number of rows for the first matrix (A) between 1 and 5.
    /// </summary>
    [SerializeField, Range(1, 7)]
    [Tooltip("Values between 1 and 7")]
    private int _rowsA;

    /// <summary>
    /// The number of columns for the first matrix (A) between 1 and 5.
    /// </summary>
    [Tooltip("Values between 1 and 5")]
    [SerializeField, Range(1, 5)]
    private int _columnsA;

    /// <summary>
    /// The number of rows for the second matrix (B) between 1 and 5.
    /// </summary>
    [Tooltip("Values between 1 and 7")]
    [SerializeField, Range(1, 7)]
    private int _rowsB;

    /// <summary>
    /// The number of columns for the second matrix (B) between 1 and 5.
    /// </summary>
    [Tooltip("Values between 1 and 5")]
    [SerializeField, Range(1, 5)]
    private int _columnsB;

    /// <summary>
    /// Stores a reference to the first matrix (A).
    /// </summary>
    private int[][] _matrixA;

    /// <summary>
    /// Stores a reference to the second matrix (B).
    /// </summary>
    private int[][] _matrixB;

    /// <summary>
    /// Stores a reference to the product of the first matrix (A) and the second matrix (B).
    /// </summary>
    private int[][] _matrixProduct;
    
    #endregion
    
    #region --Unity Specific Methods--

    private void Start()
    {
        Init();
    }
    
    #endregion
    
    #region --Custom Methods--

    /// <summary>
    /// Initializes variables and caches components.
    /// </summary>
    private void Init()
    {
        if (_columnsA != _rowsB)
        {
            Debug.Log("The column count of matrix A does not match the row count of matrix B!");
            return;
        }
        
        CreateMatrices();
        UpdateProductMatrix();
        SetUIValues();
    }

    /// <summary>
    /// Sets the values for the UI text fields.
    /// </summary>
    private void SetUIValues()
    {
        _matrixARows.SetText("{0}", _rowsA);
        _matrixAColumns.SetText("{0}", _columnsA);
        _matrixBRows.SetText("{0}", _rowsB);
        _matrixBColumns.SetText("{0}", _columnsB);
        _productMatrixRows.SetText("{0}", _rowsA);
        _productMatrixColumns.SetText("{0}", _columnsB);
    }

    /// <summary>
    /// Creates the matrices we need.
    /// </summary>
    private void CreateMatrices()
    {
        _matrixA = CreateRandomMatrix(_rowsA, _columnsA, _matrixATransform);
        _matrixB = CreateRandomMatrix(_rowsB, _columnsB, _matrixBTransform);
        _matrixProduct = CreateEmptyMatrix(_rowsA, _columnsB);
        _matrixProduct = CalculateLayeredPerspective(_matrixA, _matrixB);
    }

    /// <summary>
    /// Updates the product matrix with the final values.
    /// </summary>
    private void UpdateProductMatrix()
    {
        for (int i = 0; i < _rowsA; i++)
        {
            GameObject _row = Instantiate(_rowPrefab, _matrixProductTransform, false);
            for (int j = 0; j < _columnsB; j++)
            {
                Instantiate(_columnPrefab, _row.transform, false).GetComponent<TextMeshProUGUI>().SetText("{0}", _matrixProduct[i][j]);
            }
        }
    }

    /// <summary>
    /// Create a random matrix with the number of rows and columns specified.
    /// </summary>
    private int[][] CreateRandomMatrix(int _rows, int _columns, Transform _parent)
    {
        int[][] _matrix = new int[_rows][];

        for (int _row = 0; _row < _rows; _row++)
        {
            _matrix[_row] = new int[_columns];
            Transform _rowParent = Instantiate(_rowPrefab, _parent, false).transform;
            for (int _column = 0; _column < _columns; _column++)
            {
                _matrix[_row][_column] = Random.Range(-_range, _range);
                Instantiate(_columnPrefab, _rowParent, false).GetComponent<TextMeshProUGUI>().SetText("{0}", _matrix[_row][_column]);
            }
        }

        return _matrix;
    }

    /// <summary>
    /// Initialize an empty matrix with the number of rows and columns specified.
    /// </summary>
    private int[][] CreateEmptyMatrix(int _rows, int _columns)
    {
        int[][] _matrix = new int[_rows][];

        for (int _row = 0; _row < _rows; _row++)
        {
            _matrix[_row] = new int[_columns];
        }

        return _matrix;
    }

    /// <summary>
    /// Perform the actual matrix calculation.
    /// </summary>
    private int[][] CalculateLayeredPerspective(int[][] _matrix1, int[][] _matrix2)
    {
        int[][] _finalMatrix = CreateEmptyMatrix(_rowsA, _columnsB);
        for (int i = 0; i < _columnsA; i++)
        {
            int[][] _newMatrix = MultiplyTwoMatricesByColumn(i, _matrix1, _matrix2);
            _finalMatrix = AddTwoMatrices(_newMatrix, _finalMatrix);
        }
        
        return _finalMatrix;
    }

    /// <summary>
    /// Multiplies the two specified matrices together.
    /// </summary>
    private int[][] MultiplyTwoMatricesByColumn(int _columnValue, int[][] _matrix1, int[][] _matrix2)
    {
        int[][] _newMatrix = CreateEmptyMatrix(_rowsA, _columnsB);
        for (int _rowA = 0; _rowA < _rowsA; _rowA++)
        {
            for (int _colB = 0; _colB < _columnsB; _colB++)
            {
                _newMatrix[_rowA][_colB] = _matrix1[_rowA][_columnValue] * _matrix2[_columnValue][_colB];
            }
        }

        return _newMatrix;
    }

    /// <summary>
    /// Adds two matrices together.
    /// </summary>
    private int[][] AddTwoMatrices(int[][] _matrix1, int[][] _matrix2)
    {
        int[][] _newMatrix = CreateEmptyMatrix(_rowsA, _columnsB);
        for (int _row = 0; _row < _rowsA; _row++)
        {
            for (int _column = 0; _column < _columnsB; _column++)
            {
                _newMatrix[_row][_column] = _matrix1[_row][_column] + _matrix2[_row][_column];
            }
        }
        
        return _newMatrix;
    }
    
    #endregion
    
}
