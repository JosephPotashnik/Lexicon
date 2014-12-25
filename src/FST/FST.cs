using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteStateTransducer
{
    public class TransitionData
    {
        //the input string that is required to go through this arc.
        public string Input { get; set; }
        //the output string that is written when we traverse the arc.
        public string Output { get; set; }
        //the source state state index (0,1,... n-1)
        public int FromState { get; set; }
        //the target state index.
        public int ToState { get; set; }

        public TransitionData(string _input, string _output, int _currentState, int _nextState)
        {
            Input = _input;
            Output = _output;
            FromState = _currentState;
            ToState = _nextState;
        }
    }

    public class FSTData
    {
        //a short description of what this FST is accepting (what does it parse)
        public string Description { get; set; }
        //number of states in the FST. the states are 0,1,.... n-1.
        public int NumOfStates { get; set; }
        //the index of the accepting states.
        public List<int> acceptingStates { get; set; }
        //list of arcs
        public List<TransitionData> Arcs { get; set; }

        public FSTData()        {}

    }
    public class FST
    {
        public const string WildcardSymbol = "?";

        public int NumOfStates { get; set; }
        public string Description { get; set; }
        
        class MachineState
        {
            public bool AcceptingState { get; set; }

            List<TransitionData> transitions = new List<TransitionData>();

            public List<TransitionData> Transitions
            {
                get { return transitions; }
                set { transitions = value; }
            }
        }

        class SearchState
        {
            public int MachineState { get; set; }
            public int InputIndex { get; set; }
            public string OutputSequence { get; set; }

            public SearchState(int _machineState, int _inputIndex, string _output)
            {
                MachineState = _machineState;
                InputIndex = _inputIndex;
                OutputSequence = _output;
            }
        }


        string inputSequence;
        MachineState[] states;        

       
        public FST(FSTData data)
        {
            Description = data.Description;
            NumOfStates = data.NumOfStates;

            states = new MachineState[NumOfStates];

            for (int i = 0; i < NumOfStates; ++i)
                states[i] = new MachineState();

            foreach(int k in data.acceptingStates)
                states[k].AcceptingState = true;


            foreach(TransitionData arc in data.Arcs )
                states[arc.FromState].Transitions.Add(arc);
        }

        public void AddArcToModel(string input, string output, int currentState, int nextState)
        {
            TransitionData d = new TransitionData(input, output, currentState, nextState);
            states[currentState].Transitions.Add(d);

        }

        public void SwitchBetweenInputAndOutput()
        {
            for (int i = 0; i < NumOfStates; ++i)
            {
                foreach (TransitionData d in states[i].Transitions)
                {
                    string output = d.Output;
                    d.Output = d.Input;
                    d.Input = output;
                }
            }
        }

       

        //Parser of FST acts as a generator of the output.
        public List<string> ParseInput(string input)
        {
            List<string> answers = new List<string>();
            inputSequence = input;

            List<SearchState> agenda = new List<SearchState>();

            //pointing at beginning of states and beginning of input sequence list.
            SearchState currentSearchState = new SearchState(0, 0, null);
            agenda.Add(currentSearchState);

            while (agenda.Count != 0)
            {
                if (IsAcceptState(currentSearchState) == true)
                {
                    answers.Add(currentSearchState.OutputSequence);
                }
                else
                {
                    List<SearchState> nextStates = GenerateNewSearchStates(currentSearchState);
                    foreach(SearchState s in nextStates)
                        agenda.Add(s);

                }

                currentSearchState = GetNextSearchState(agenda);
            }

            return answers;
        }

        //Using BFS - breadth search first, remove the current search state and get the first one in line.
        SearchState GetNextSearchState(List<SearchState> agenda)
        {
            SearchState s = null;
            agenda.RemoveAt(0);
           
            if (agenda.Count != 0)
                s =  agenda[0];

            return s;
        }

        bool IsAcceptState(SearchState s)
        {
            return ((states[s.MachineState].AcceptingState == true) && (s.InputIndex == inputSequence.Count()));
        }
      
        List<SearchState> GenerateNewSearchStates(SearchState s)
       {
           int currentState = s.MachineState;
           List<SearchState> newSearchStates = new List<SearchState>();

            //if we have not yet exceeded the input length
           if (s.InputIndex < inputSequence.Count())
           {

               foreach (TransitionData d in states[currentState].Transitions)
               {

                   //if there is a transition whose input is null (epsilon transition)
                   if (d.Input == null)
                   {
                       //the new search state does not advance the index in the input sequence (epsilon transition)
                       SearchState nextSearchState = CreateNextSearchState(s, d, s.InputIndex, s.InputIndex);
                       newSearchStates.Add(nextSearchState);

                   }
                   else
                   {
                       //if the symbol on the input string equals the input symbol on an arc leading out of the current state, 
                       //or the transition accepts any character (i.e. '?' wildcard)
                       if (d.Input == inputSequence[s.InputIndex].ToString() || d.Input.Equals(WildcardSymbol))
                       {
                           //advance the input sequence and move to next search state.
                           SearchState nextSearchState = CreateNextSearchState(s, d, s.InputIndex + 1, s.InputIndex);
                           newSearchStates.Add(nextSearchState);
                       }
                   }
               }
           }

            return newSearchStates;

        }

        SearchState CreateNextSearchState(SearchState s, TransitionData d, int nextInputIndex, int currentInputIndex)
        {
            string output = null;

            //crteate a new output sequence
            if (s.OutputSequence != null)
                output = s.OutputSequence;

            //write the output symbol on the arc to the outputsequence.
            if (d.Output != null)
            {
                
                string outputSymbol = d.Output;

                //if output symbol is ? (wildcard), then copy the actual input from the input sequence.
                if (outputSymbol.Equals(WildcardSymbol))
                    outputSymbol = inputSequence.ElementAt(currentInputIndex).ToString();

                output+= outputSymbol;
             }
            //create a new search state and add it to the list.
            //the new search state has the next index in the input sequence
            return new SearchState(d.ToState, nextInputIndex, output);

        }
    }
}
