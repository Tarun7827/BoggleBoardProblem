
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoggleBoard : MonoBehaviour
{
	int rowCount, colCount;

	//Stores all words provided by user
	string[] allWords;

    //Contains each cell value from board
	string[,] boggleGrid;

	//Dictionary to collect each word and to be used for checking the existence
	public Dictionary<string, string> dictionary = new Dictionary<string, string>();	

	[Header("Input Objects")]
	public InputField rows;
	public InputField columns;
	public InputField dictionaryInput;

	[Header("Grid Objects")]
	public InputField cell;
	public Transform gridObject;
	public List<InputField> gridCells;

	[Header("Output Objects")]
	public Text foundWords;	
	public Text foundWordsCountTxt;

	//Keeps track of found words count
	int foundWordsCount = 0;

	[Space]
	//total words Provided by user
	public Text wordsInBoardTxt;     

	#region InputMethods

	/// <summary>
	/// Sends a call to create Board
	/// Rows and Columns count must be same
	/// </summary>
	public void CreateGrid()
	{
		if(rows.text == columns.text)
		{
			rowCount = int.Parse(rows.text);
			colCount = int.Parse(columns.text);
			GridInputs(rowCount, colCount);
		}
	}

	/// <summary>
	/// Creates the Board with Dimension Inputs
	/// </summary>
	/// <param name="rows"></param>
	/// <param name="columns"></param>
	void GridInputs(int rows, int columns)
	{
		//Check and Destroy existing cells
		for (int i = 0; i < gridObject.childCount; i++)
		{
			Destroy(gridObject.GetChild(i).gameObject);
		}

		gridCells.Clear();
		//Instantiate each cell and store as InputFields in the List		
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				gridCells.Add(Instantiate(cell, gridObject));				
			}
		}
	}

	/// <summary>
	/// To be called after user provides all the inputs to each cell
	/// </summary>
	public void SubmitGridValues()
	{
		int index = 0;
		boggleGrid = new string[rowCount, colCount];

		//Store each cell value in 2D grid
		for (int i = 0; i < rowCount; i++)
		{
			for (int j = 0; j < colCount; j++)
			{
				boggleGrid[i, j] = gridCells[index++].text;			
			}
		}
	}

	/// <summary>
	/// To be called after the User provides all the words to be found
	/// Stores each word in dictionary and Trims if any space exist
	/// </summary>
	public void SubmitDictionary()
	{		
		allWords = dictionaryInput.text.Split(',');
		
		//Clear dictionary if user provides some inputs again
		//As user can add or remove any word
		dictionary.Clear();
		for (int i = 0; i < allWords.Length; i++)
		{
			allWords[i] = allWords[i].Trim();
			dictionary.Add(allWords[i], allWords[i]);
		}

		wordsInBoardTxt.text = "TOTAL WORDS : " + dictionary.Count;

	}

	#endregion

	#region OutputMethods

	/// <summary>
	/// To be called after User provides Grid and Dictionary of words	
	/// </summary>
	public void TraverseGrid()
	{
		foundWordsCount = 0;
		foundWordsCountTxt.text = "TOTAL WORDS FOUND : " + foundWordsCount;
		foundWords.text = "";
		//Marks each cell as non-visited
		bool[,] visited = new bool[rowCount, colCount];
		string str = "";

		//Starting with each grid element
		for (int i = 0; i < rowCount; i++)
		{
			for (int j = 0; j < colCount; j++)
			{
				FindWord(i, j, str, visited);
			}
		}
	}

	/// <summary>
	/// Check for the word and Traverse adjacent cells for find more words
	/// with Inputs as row number, column number, current string, visited character grid respectively
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="str"></param>
	/// <param name="visited"></param>
	void FindWord(int i, int j, string str, bool[,] visited)
	{
		//Marks current cell as visited to avoid reusing any character
		visited[i, j] = true;

		str += boggleGrid[i, j];

		//Contraint that were provided if to check words of length 4 or more
		if (str.Length >= 4)
		{
			//If current string exists then
			//Display in Output
			//Update counter of total words
			if (IsWord(str))
			{
				foundWords.text += (str + '\n');
				foundWordsCount++;
				foundWordsCountTxt.text = "TOTAL WORDS FOUND : " + foundWordsCount;
			}
		}

		//Clamps row between 0 and rowcount
		int minr = Mathf.Clamp(i - 1, 0, rowCount - 1);
		int maxr = Mathf.Clamp(i + 1, 0, rowCount - 1);

		//Clamps row between 0 and columnCount
		int minc = Mathf.Clamp(j - 1, 0, colCount - 1);
		int maxc = Mathf.Clamp(j + 1, 0, colCount - 1);

		//Traversing each adjacent cell 
		for (int row = minr; row <= maxr; row++)
		{
			for (int col = minc; col <= maxc; col++)
			{
				//Calling FindWords for no-visited cells only
				if (!visited[row, col])
					FindWord(row, col, str, visited);
			}
		}

		//Marking current cell visited as false
		str = "" + str[str.Length - 1];
		visited[i, j] = false;

	}

	/// <summary>
	/// Returns whether current string exists in dictionary or not
	/// </summary>
	/// <param name="str"></param>
	/// <returns></returns>
	bool IsWord(string str)
	{
		return dictionary.ContainsKey(str);
	}

	#endregion
}
