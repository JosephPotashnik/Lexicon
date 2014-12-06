using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FiniteStateTransducer;

namespace Lexicon
{
    
    public class Lexicon
    {

      
        public Lexicon()
        {

            FST fstPast = new FST();


            fstPast.AddArcToModel("?", "?", 0, 0);
            fstPast.AddArcToModel("ת", null, 0, 1);
            fstPast.AddArcToModel("י", "[singular][past][1person]", 1, 2);
            fstPast.SetAcceptingState(2);
            fstPast.AddArcToModel("ת", "[singular][past][2person]", 0, 3);
            fstPast.SetAcceptingState(3);
            fstPast.AddArcToModel("ה", "[singular][past][3person][feminine]", 0, 4);
            fstPast.SetAcceptingState(4);
            fstPast.AddArcToModel("נ", null, 0, 5);
            fstPast.AddArcToModel("ו", "[plural]tata[past][1person]", 5, 6);
            fstPast.SetAcceptingState(6);
            fstPast.AddArcToModel("ם", "[plural][past][2person][masculine]", 1, 7);
            fstPast.AddArcToModel("ן", "[plural][past][2person][feminine]", 1, 8);
            fstPast.SetAcceptingState(7);
            fstPast.SetAcceptingState(8);
            string s1 = "למדתן";
            List<string> outputs1 = fstPast.ParseInput(s1);

           Lexeme lex = new Lexeme(outputs1[0]);

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


 
        }
        
    }
}
