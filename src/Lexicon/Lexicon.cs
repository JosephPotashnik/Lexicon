using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using FiniteStateTransducer;

namespace Lexicon
{
    public class FeatureDictionary
    {
        public Dictionary<string, int> Dictionary { get; set; }
        public int NumOfFeatures { get; set; }

        public FeatureDictionary(FeatureDefinitions defs)
        {
            NumOfFeatures = defs.FeatureNamesVector.FeatureNames.Count();
            Dictionary = new Dictionary<string, int>();
            foreach (Feature f in defs.Features)
            {
                foreach (string s in f.Values)
                {
                    //foreach(string name in defs.FeatureNamesVector.FeatureNames)
                    for (int k = 0; k < NumOfFeatures; ++k)
                    {
                        if (f.Name == defs.FeatureNamesVector.FeatureNames[k])
                        {
                            Dictionary.Add(s, k);
                            break;
                        }
                    }
                }

            }
        }
    }

    public class Lexicon
    {
      
        public Lexicon()
        {
            //read feature space
            string featurDefs = File.ReadAllText(@"..\..\..\..\config\FeaturesDefinitions.json");
            FeatureDefinitions defs = JsonConvert.DeserializeObject<FeatureDefinitions>(featurDefs);

            FeatureDictionary dic = new FeatureDictionary(defs);

            //generate FST machines for morphological parse
            string fstContent = File.ReadAllText(@"..\..\..\..\config\FSTDefinitions.json");
            FSTData data = JsonConvert.DeserializeObject<FSTData>(fstContent);
            FST fstPast = new FST(data);

            //parse!
            string input = "למדתן";
            List<string> outputs1 = fstPast.ParseInput(input);

            //generate lexeme.

            Lexeme lex = new Lexeme(outputs1[0], input, dic);
            string tutu = JsonConvert.SerializeObject(lex, Formatting.Indented);
            string dir = Directory.GetCurrentDirectory();
            File.WriteAllText(@"..\..\..\..\config\tutu.json", tutu);

        }     
    }
}

/*
FST fstFuture = new FST();

fstFuture.AddArcToModel("א", "[1person][future][singular]", 0, 1);
fstFuture.AddArcToModel("?", "?", 1, 1);
fstFuture.SetAcceptingState(1);

string s2 = "אלמד";
List<string> outputs2 = fstFuture.ParseInput(s2);


FST fstHifil = new FST();

fstHifil.AddArcToModel("ה", null, 0, 1);
fstHifil.AddArcToModel("?", "?", 1, 2);
fstHifil.AddArcToModel("?", "?", 2, 2);
fstHifil.AddArcToModel("י", "[hifil]", 2, 3);
fstHifil.AddArcToModel("?", "?", 3, 4);
fstHifil.SetAcceptingState(4);


string s3 = "הרכיב";
List<string> outputs3 = fstHifil.ParseInput(s3);

Lexeme lex3 = new Lexeme(outputs3[0]);
*/