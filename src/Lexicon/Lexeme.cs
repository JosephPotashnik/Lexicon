﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Lexicon
{
    class Lexeme
    {
        string orthographicStem;
        FeaturesVector features;
        //List<string> phoneticExpressions;
        //List<string> meanings; - list or tree of meanings (to represent homonymy/polysemy)

        //the constructor receives the output of the FST. FST receives surface form and returns a string such that:
        //a. substring surrounded by brackets are values of some morphological feature.
        //b. what is not surrounded by brackets is part of the orthographic stem (Hebrew verbal root).
        public Lexeme(string parsedOutput)
        {

            //remove all morphological marking from the parse string.
            orthographicStem = Regex.Replace(parsedOutput, @"\[(\w+)\]", "");

            //get all morphological marking
            MatchCollection mc = Regex.Matches(parsedOutput, @"\[(\w+)\]", RegexOptions.IgnorePatternWhitespace);
            for (int i = 0; i < mc.Count; ++i)
            {

                string feature = mc[i].ToString();
            }
        }
    }
}