﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralTools
{
    class Tools
    {
        public Tools() { }

        public List<string> FindMinMaxPerMember(List<string> SourceArray)
        {
            List<string> MinMaxValues = new List<string>();

            return MinMaxValues;
        }
        public List<string> ParseFileToResultArray(string FilePath, string ElementPrefix, string ResultType)
        {
            string[] lines = LoadTextFile(FilePath);
            List<string> SortedLines = SeparateLinesWithValues(lines, ElementPrefix, ResultType);
            string[] HeaderCells = SeparateTableCells(SortedLines[0]);

            int ValuePosition = -1;
            for (int i = 0; i < HeaderCells.Length; i++)
            {
                if (HeaderCells[i].Contains(ResultType))
                {
                    ValuePosition = i;
                    break;
                }
            }
            List<string> ResultValues = new List<string>();
            foreach (string Line in SortedLines)
            {
                string[] ResultCells = SeparateTableCells(Line);
                if (!Line.Contains(SortedLines[0]))
                {
                    string SingleResultValue = RemoveNotesAfterValues(ResultCells[ValuePosition]);
                    ResultValues.Add(SingleResultValue);
                }
            }
            return ResultValues;
        }
        private string[] LoadTextFile(string FilePath)
        {
            string[] lines = File.ReadAllLines(FilePath, Encoding.UTF8);
            return lines;
        }
        private List<string> SeparateLinesWithValues(string[] AllLines, string BeamIndexing, string ResultType)
        {
            List<string> lst = new List<string>();
            int LineHeader = -1;

            for (int i = 0; i < AllLines.Length; i++)
            {
                string ActLine = AllLines[i];

                if (ActLine.Contains(ResultType))
                {
                    LineHeader = i;
                    lst.Add(ActLine);

                    i++;
                    ActLine = AllLines[i];
                }
                if (LineHeader > -1)
                {
                    if (string.IsNullOrEmpty(ActLine)) { break; }
                    else
                    {
                        if (ActLine.Contains(BeamIndexing))
                        {
                            lst.Add(ActLine);
                        }
                        else { break; }
                    }
                }
            }
            return lst;
        }
        private string[] RemoveEmptyArrayEntries(string[] Array)
        {
            var temp = new List<string>();
            foreach (var s in Array)
            {
                if (!string.IsNullOrEmpty(s))
                    temp.Add(s);
            }
            return temp.ToArray();
        }
        private string[] SeparateTableCells(string SingleLine)
        {
            string[] cells = SingleLine.Split('\t');
            return cells;
        }
        private string RemoveNotesAfterValues(string SingleCell)
        {
            string[] cells = SingleCell.Split(' ');
            return cells[0];
        }
    }
}
