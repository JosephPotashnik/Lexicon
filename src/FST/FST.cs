using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiniteStateTransducer
{
    public class FST
    {
        int MaxStates = 10;

        class TransitionData
        {
            string input;

            public string Input
            {
                get { return input; }
                set { input = value; }
            }
            string output;

            public string Output
            {
                get { return output; }
                set { output = value; }
            }
            int nextState;

            public int NextState
            {
                get { return nextState; }
                set { nextState = value; }
            }

            public TransitionData(string _input, string _output, int _nextState) 
            {
                input = _input;
                output = _output;
                nextState = _nextState;
            }
        }

        class MachineState
        {
            bool acceptingState = false;

            public bool AcceptingState
            {
                get { return acceptingState; }
                set { acceptingState = value; }
            }
            List<TransitionData> transitions = new List<TransitionData>();

            public List<TransitionData> Transitions
            {
                get { return transitions; }
                set { transitions = value; }
            }
            public void SetAcceptingMachineState() { acceptingState = true; }
        }

        class SearchState
        {
            int machineState;

            public int MachineState
            {
                get { return machineState; }
                set { machineState = value; }
            }
            int inputIndex;

            public int InputIndex
            {
                get { return inputIndex; }
                set { inputIndex = value; }
            }

            string outputSequence;

            public string OutputSequence
            {
                get { return outputSequence; }
                set { outputSequence = value; }
            }

            public SearchState(int _machineState, int _inputIndex, string _output)
            {
                machineState = _machineState;
                inputIndex = _inputIndex;
                outputSequence = _output;

            }

        }


        string inputSequence;
        MachineState[] states;   //assumption: the states correspond to parsing of the input; thus number of states is max number of characters.        
        public void SetAcceptingState(int state) { states[state].SetAcceptingMachineState(); }

        public FST()
        {
            //assumption: the states correspond to parsing of the input; thus number of states is max number of characters.        
            states = new MachineState[MaxStates];
            for (int i = 0; i < MaxStates; ++i)
            {
                MachineState s = new MachineState();
                states[i] = s;
            }
        }

        public void AddArcToModel(string input, string output, int currentState, int nextState)
        {
            TransitionData d = new TransitionData(input, output, nextState);
            states[currentState].Transitions.Add(d);

        }

        public void SwitchBetweenInputAndOutput()
        {
            for (int i = 0; i < MaxStates; ++i)
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
                       if (d.Input == inputSequence[s.InputIndex].ToString() || d.Input.Equals("?"))
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
                output = String.Copy(s.OutputSequence);

            //write the output symbol on the arc to the outputsequence.
            if (d.Output != null)
            {
                ;
                string outputSymbol = d.Output;

                //if output symbol is ? (wildcard), then copy the actual input from the input sequence.
                if (outputSymbol.Equals("?"))
                    outputSymbol = inputSequence.ElementAt(currentInputIndex).ToString();

                output+= outputSymbol;
             }
            //create a new search state and add it to the list.
            //the new search state has the next index in the input sequence
            return new SearchState(d.NextState, nextInputIndex, output);

        }
    }
}
