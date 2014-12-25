using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using FiniteStateTransducer;
using System.Xml;
using System.Text.RegularExpressions;
using System.Collections;

namespace Lexicon
{
    public class Lexicon
    {
        public const string FeatureDefinitionFile = @"..\..\..\..\config\FeaturesDefinitions.json";
        public const string CliticDefinitionFile = @"..\..\..\..\config\CliticsDefinitions.json";
        public const string FSTDirectory = @"..\..\..\..\config\Morphological FST";


        FeatureDefinitions defs;
        Clitic[] clitics;
        FST[] Fsts;
        MultiValueDictionary<string, Lexeme> lexicon;
     
        FST[] ReadFSTs()
        {
            //generate FST machines for morphological parse
            string[] files = Directory.GetFiles(FSTDirectory, "*.json");
            int numOfFst = files.Count();

            FST[] Fsts = new FST[numOfFst];

            for (int k = 0; k < numOfFst; ++k)
            {
                string fstContent = File.ReadAllText(files[k]);
                FSTData data = JsonConvert.DeserializeObject<FSTData>(fstContent);
                Fsts[k] = new FST(data);
            }
            return Fsts;
        }

        List<Lexeme> Parse(string input)
        {
            List<Lexeme> possibleInterpretations = new List<Lexeme>();

            foreach(FST f in Fsts)
            {
                List<string> parses = f.ParseInput(input);
                foreach (string p in parses)
                {
                    Lexeme lex = new Lexeme(p, input);
                    possibleInterpretations.Add(lex);
                }

            }

            return possibleInterpretations;
        }

        List<string> TokenizeClitics(string input)
        {
            List<string> inputs = new List<string>();
            inputs.Add(input);

            foreach (Clitic c in clitics)
            {
                if (c.Direction == CliticDireciton.Prefix)
                {
                    if (input.StartsWith(c.Value))
                        inputs.Add(input.Substring(c.Value.Count()));
                }
                else
                {
                    if (input.EndsWith(c.Value))
                        inputs.Add(input.Substring(0, input.Count() - c.Value.Count()));
                }
            }
            return inputs;
        }
        
        public class WiktionaryNameValuePair
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public class ListWiktionaryNameValuePair
        {
            public List<WiktionaryNameValuePair> DicList { get; set; }
        }

        private void TraverseNodes(XmlNodeList children, ListWiktionaryNameValuePair list)
        {
            foreach(XmlNode child in children)
            {
                
                if (child.ChildNodes[0].LocalName == "title")
                {
                    string s = child.ChildNodes[0].ChildNodes[0].Value;
                    if (s != null &&  !s.Contains(':'))
                    {

                        WiktionaryNameValuePair d = new WiktionaryNameValuePair();
                        d.Name = s;
                        XmlNode revisionNode = child.ChildNodes[3];
                        foreach (XmlNode node in revisionNode)
                        {
                            if (node.Name == "text")
                            {
                                d.Value = node.ChildNodes[0].Value;
                                list.DicList.Add(d);
                            }
                        }
                    }
                }


            }
           
        }
        public ListWiktionaryNameValuePair ExtractWiktionaryTextFromWiktionaryXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\Sefi\Documents\Visual Studio 2013\Projects\Lexicon\config\hewiktionary-20141124-pages-articles-multistream.xml");
            XmlElement  root = doc.DocumentElement;
            ListWiktionaryNameValuePair list = new ListWiktionaryNameValuePair();
            list.DicList = new List<WiktionaryNameValuePair>();
            TraverseNodes(root.ChildNodes, list);
            return list;
            
        }

        private ListWiktionaryNameValuePair ExtractFeaturesTextFromWiktionaryText(ListWiktionaryNameValuePair items)
        {
            ListWiktionaryNameValuePair newList = new ListWiktionaryNameValuePair();
            newList.DicList = new List<WiktionaryNameValuePair>();

            for(int j=0;j<items.DicList.Count;++j) 
            {
                
                int openBrackets = 0;
                List<string> t = new List<string>();

                int startIndex = 0;
                for (int k = 0; k < items.DicList[j].Value.Count(); ++k)
                {
                    if (items.DicList[j].Value[k] == '{')
                    {
                        if (openBrackets == 0)
                            startIndex = k;

                        openBrackets++;
                    }

                    //what we want from the value nodes are the outermost sub-strings that look like that : {{ * }} (they may be nested, we do not want the nested ones)
                    if (items.DicList[j].Value[k] == '}')
                    {

                        if (openBrackets == 1 && k - startIndex > 3)
                        {
                            string check = items.DicList[j].Value.Substring(startIndex + 2, k - startIndex - 3);
                            if (check.Contains("ניתוח דקדוקי"))
                            {
                                //Console.WriteLine("index {0} in item {1} out of {2} items", k, j, items.DicList.Count);
                                WiktionaryNameValuePair newVal = new WiktionaryNameValuePair();
                                newVal.Name = items.DicList[j].Name;
                                newVal.Value = check;
                                newList.DicList.Add(newVal);
                            }

                        }
                        openBrackets--;
                    }
                }
            }

            return newList;
        }

        private MultiValueDictionary<string, Lexeme> GenerateMorphologicalVectorFromText(ListWiktionaryNameValuePair items)
        {
            MultiValueDictionary<string, Lexeme> dic = new MultiValueDictionary<string, Lexeme>();
            char[] sep = {'|', '\n' };


            foreach (WiktionaryNameValuePair item in items.DicList)
            {

                string[] s = item.Value.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                string FeatureVecString = string.Empty;
                string featureValue = null;

                string itemFullName = null;
                for(int k=0;k<s.Count();++k)
                {
                    //I am forced to take particular care of parsing in each case because the spelling of the wikitionary authors vary drastically.
                    //some use just "שם", others "שם-עצם", with or without spaces, etc.
                    if (s[k].Contains("חלק דיבר"))
                    {
                        string[] s1 = s[k].Split('=');
                        if (s1.Count() == 2)
                        {
                            featureValue = s1[1];

                            if (featureValue.Contains(@"עצם") || featureValue.Contains(@"צרף") || featureValue.Contains(@"צרוף")
                                || featureValue.Contains(@"שם"))
                                FeatureVecString += "[Noun]";

                            if (featureValue.Contains(@"תואר") || featureValue.Contains(@"תאר"))
                                FeatureVecString += "[Adjective]";

                            if (featureValue.Contains(@"פועל") || featureValue.Contains(@"פעל"))
                                FeatureVecString += "[Adjective]";

                        }
                    }

                    if (s[k].Contains("מין"))
                    {
                        string[] s1 = s[k].Split('=');
                        if (s1.Count() == 2)
                        {
                            featureValue = s1[1];

                            if (featureValue.Contains(@"ז"))
                                FeatureVecString += "[Masculine]";

                            if (featureValue.Contains(@"נ"))
                                FeatureVecString += "[Feminine]";

                        }
                    }

                    if (s[k].Contains("בניין"))
                    {
                        string[] s1 = s[k].Split('=');
                        if (s1.Count() == 2)
                        {
                            featureValue = s1[1];

                            if (featureValue.Contains(@"פעל") || featureValue.Contains(@"קל") || featureValue.Contains(@"פָּעַל"))
                                FeatureVecString += "[Verb][Paal]";

                            if (featureValue.Contains(@"פיעל") || featureValue.Contains(@"פִּעֵל"))
                                FeatureVecString += "[Verb][Piel]";

                            if (featureValue.Contains(@"פועל") || featureValue.Contains(@"פֻּעַל"))
                                
                            if (featureValue.Contains(@"נפעל"))
                                FeatureVecString += "[Verb][Nifal]";

                            if (featureValue.Contains(@"התפעל"))
                                FeatureVecString += "[Verb][Hitpael]";

                            if (featureValue.Contains(@"הפעיל"))
                                FeatureVecString += "[Verb][Hifil]";

                        }
                    }

                    if (s[k].Contains("כתיב מלא"))
                    {
                        string[] s1 = s[k].Split('=');
                        if (s1.Count() == 2)
                        {
                            //sometimes wiktionary authors put multiple reading instructions and references, ignore that.
                            if (!s1[1].Contains(@"["))
                            {
                                FeatureVecString += s1[1];
                                itemFullName = s1[1];
                            }

                        }
                    }
                    
                }

                //if the field כתיב מלא is available, use it.
                string name = itemFullName;
                if (name == null)
                    name = item.Name;      
                
                Lexeme l = new Lexeme(FeatureVecString, name);
                dic.Add(name, l);
            }

            return dic;
        }

        private MultiValueDictionary<string, Lexeme> GenerateJsonFromWiktionary()
        {
            //copying the text of the relevant nodes from the XML file into a List structure.
            ListWiktionaryNameValuePair list = ExtractWiktionaryTextFromWiktionaryXML();

            //extracting the relevant text from the nodes.
            ListWiktionaryNameValuePair newList = ExtractFeaturesTextFromWiktionaryText(list);

            //read Dictionary
            MultiValueDictionary<string, Lexeme> dic = GenerateMorphologicalVectorFromText(newList);

            //in addition, write to JSON file:
            string lexicon = JsonConvert.SerializeObject(dic,  Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(@"d:\lexicon.json", lexicon);


            return dic;
        }

        private void ReadDefinitions()
        {

            //read feature space
            string featurDefs = File.ReadAllText(FeatureDefinitionFile);
            defs = JsonConvert.DeserializeObject<FeatureDefinitions>(featurDefs);

            //prepare dictionary translating from feature value to vector index. (e.g. "Person" = index 0 in features vector, "Number" = index 2, etc..).
            FeatureDictionary dic = new FeatureDictionary(defs);
            Lexeme.FeatureDic = dic;

            //the following line is in comments. it is used only once, when generating new dictionary from wikitionary dump.
            //lexicon = GenerateJsonFromWiktionary();

            //read the lexicon from file until I establish DB.
            string lexiconString = File.ReadAllText(@"d:\lexicon.json");
            lexicon = JsonConvert.DeserializeObject<MultiValueDictionary<string, Lexeme>>(lexiconString);

            //read clitics
            string cliticDefs = File.ReadAllText(CliticDefinitionFile);
            clitics = JsonConvert.DeserializeObject<Clitic[]>(cliticDefs);

            //read Fsts
            Fsts = ReadFSTs();
        }

        public Lexicon()
        {
            //read initial data structures: feature space, clitics, State machines, Lexicon.
            ReadDefinitions();

            //parse!
            string input = "אלמד";
            List<string> inputs = TokenizeClitics(input);

            List<Lexeme> list = Parse(input);

            foreach(Lexeme l in list)
            {
                if (lexicon.ContainsKey(l.OrthographicStem))
                {

                    //found in lexicon!!
                    IReadOnlyCollection<Lexeme> tutuuuuuu = lexicon[l.OrthographicStem];
                    int x = 1;

                }
            }

            input = "למדתי";
            inputs = TokenizeClitics(input);

            list = Parse(input);
            input = "אלמדתי";
            list = Parse(input);

        }     
    }
}