using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
[ExecuteAlways]
public class FileParser : MonoBehaviour
{
    public TextAsset data;
    public bool ClickMeToParse;

    [Header("ParsingSettings")]
    public bool isFocusingWindow = false;



    private void Start()
    {
        ClickMeToParse = false;
    }
    private void Update()
    {
        if (ClickMeToParse)
        {
            Parse();
            ClickMeToParse = false;
        }
    }

    public void Parse()
    {
        string text = data.text;
        string[] dialogsToParse = text.Split('#');

        if (!AssetDatabase.IsValidFolder("Assets/Dialogs/" + data.name))
            AssetDatabase.CreateFolder("Assets/Dialogs", data.name);



        for (int i = 0; i < dialogsToParse.Length; i++)
        {
            string cur = dialogsToParse[i];
            string[] lines = cur.Split('\n');

            Dialog dialog = ParseDialog(lines);



            //string name = AssetDatabase.GenerateUniqueAssetPath("Assets/Dialogs/" + data.name + "/" + data.name + " " + i + ".asset");
            string name = "Assets/Dialogs/" + data.name + "/" + data.name + " " + i + ".asset";
            if (!File.Exists(name))
                AssetDatabase.CreateAsset(dialog, name);
            else
            {
                Dialog old = (Dialog)AssetDatabase.LoadAssetAtPath(name, typeof(Dialog));
                old.Copy(dialog);
                EditorUtility.SetDirty(old);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (isFocusingWindow)
                EditorUtility.FocusProjectWindow();
        }
    }

    private static Dialog ParseDialog(string[] lines)
    {
        Dialog dialog = ScriptableObject.CreateInstance<Dialog>();


        dialog.lines = new List<string>();
        dialog.requireTags = new List<Tags>();
        dialog.bubbleLines = new List<BubbleLine>();
        dialog.requirementFrom = PersonShablon.GenerateEmptyShablon();
        dialog.requirementTo = PersonShablon.GenerateEmptyShablon();

        bool inDialog = false;
        bool inOperator = false;
        bool inBubbleLines = false;
        for (int j = 0; j < lines.Length; j++)
        {
            if (lines[j].IndexOf("//") == 0)
                continue;

            if (lines[j].Contains("ID:"))
            {
                inDialog = false;
                inOperator = false;
                dialog.Id = int.Parse(lines[j].Remove(lines[j].IndexOf("ID:"), "ID:".Length));
            }
            if (lines[j].Contains("UNLOCK:"))
            {
                inDialog = false;
                inOperator = false;
                string line = lines[j].Remove(lines[j].IndexOf("UNLOCK:"), "UNLOCK:".Length);
                
                
                line = line.Trim();

                string[] unparsed = line.Split(" ",StringSplitOptions.RemoveEmptyEntries);
                dialog.requireTags = new List<Tags>();
                dialog.forbiddenTags = new List<Tags>();
                for (int i = 0; i < unparsed.Length; i++)
                {
                    if (unparsed[i] == "")
                        continue;

                    
                    if(unparsed[i][0] == '!')
                    {
                        string inverted = unparsed[i].Substring(1);
                        dialog.forbiddenTags.Add((Tags)(Enum.Parse(typeof(Tags), inverted)));
                    }
                    else
                    {
                        dialog.requireTags.Add((Tags)(Enum.Parse(typeof(Tags), unparsed[i])));
                    }
                       

                }
            }
            if (lines[j].Contains("FROM:"))
            {
                inDialog = false;
                inOperator = false;
                string tagsLine = lines[j].Remove(lines[j].IndexOf("FROM:"), "FROM:".Length);
                string[] tags = tagsLine.Split(' ');
                for (int k = 0; k < tags.Length; k++)
                {
                    if (Enum.TryParse(tags[k], true, out Work work))
                        dialog.requirementFrom.Work = work;
                    if (Enum.TryParse(tags[k], true, out Temperament temperament))
                        dialog.requirementFrom.Temperament = temperament;
                    if (Enum.TryParse(tags[k], true, out Age age))
                        dialog.requirementFrom.Age = age;
                    if (Enum.TryParse(tags[k], true, out Sex sex))
                        dialog.requirementFrom.Sex = sex;
                    if (int.TryParse(tags[k], out int roomNumber))
                        dialog.requirementFrom.roomNumber = roomNumber - 1;
                }
            }
            if (lines[j].Contains("TO:"))
            {
                inDialog = false;
                inOperator = false;
                string tagsLine = lines[j].Remove(lines[j].IndexOf("TO:"), "TO:".Length);
                string[] tags = tagsLine.Split(' ');
                for (int k = 0; k < tags.Length; k++)
                {
                    if (Enum.TryParse(tags[k], true, out Work work))
                        dialog.requirementTo.Work = work;
                    if (Enum.TryParse(tags[k], true, out Temperament temperament))
                        dialog.requirementTo.Temperament = temperament;
                    if (Enum.TryParse(tags[k], true, out Age age))
                        dialog.requirementTo.Age = age;
                    if (Enum.TryParse(tags[k], true, out Sex sex))
                        dialog.requirementTo.Sex = sex;
                    if (int.TryParse(tags[k], out int roomNumber))
                        dialog.requirementTo.roomNumber = roomNumber - 1;
                }
            }
            if (lines[j].Contains("OPERATOR:"))
            {
                inOperator = true;
                inDialog = false;
                inBubbleLines = false;

            }
            if (lines[j].Contains("DIALOG:"))
            {
                inDialog = true;
                inOperator = false;
                inBubbleLines = false;
                continue;
            }
            if (lines[j].Contains("BUBBLES:"))
            {
                inDialog = false;
                inOperator = false;
                inBubbleLines = true;
                continue;
            }
            if (inOperator)
            {
                try
                {
                    if (lines[j].Length > 0)
                    {
                        if (lines[j][0] == '-')
                        {
                            dialog.SayToOperator += "\n" + lines[j].Remove(0, 1);
                        }
                        else
                        {
                            string line = lines[j];

                            if (line.Contains("OPERATOR:"))
                                line = line.Remove(lines[j].IndexOf("OPERATOR:"), "OPERATOR:".Length);
                            if (line.Length > 0)
                                dialog.SayToOperator = line;
                        }
                    }
                }
                catch
                {
                    Debug.Log("Crash at: dialog " + 2 + " line " + dialog.lines.Count);
                }
                continue;
            }

            if (inDialog)
            {
                try
                {
                    if (lines[j].Length > 0)
                    {
                        if (lines[j][0] == '-')
                        {
                            dialog.lines[dialog.lines.Count - 1] += "\n" + lines[j].Remove(0, 1);
                        }
                        else
                        {
                            dialog.lines.Add(lines[j]);
                        }
                    }
                }
                catch
                {
                    Debug.Log("Crash at: dialog " + 2 + " line " + dialog.lines.Count);
                }
                continue;
            }
            if (inBubbleLines)
            {
                try
                {
                    if (lines[j].Length > 0)
                    {
                        string line = lines[j].Substring(2);
                        BubbleLine bl = new BubbleLine();
                        bl.lineIndex = int.Parse("" + lines[j][0]);
                        bl.bubble = line;
                        dialog.bubbleLines.Add(bl);

                    }
                }
                catch
                {
                    Debug.Log("Crash at: dialog " + 3 + " line " + dialog.lines.Count);
                }
                continue;
            }

        }
        return dialog;
    }
}
#endif