using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexicon
{
    public enum CliticDireciton
    {
        Prefix,
        Postfix,
    }

    public class Clitic : Lexeme
    {

        public CliticDireciton Direction { get; set; }
        public string Description { get; set; }

        //string of the clitic features. e.g. "[Preposition]", or "[Determiner]".
        public string Features 
        {
             set { base.FillFeatureVec(value); }
        }

        public string Value
        {
            get { return base.OrthographicStem;  }
            set { base.OrthographicStem = base.OrthographicSurfaceForm = value;}
        }

    }
}
