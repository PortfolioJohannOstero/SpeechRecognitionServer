using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Speech.Recognition;
using System.Speech.Synthesis;

using SpeechServer.XML;

using System.Threading;
using System.Globalization;

namespace SpeechToCommand
{
    class SpeechCommandRecogniser
    {
        SpeechRecognitionEngine mRecogniser = null;
        string mUtterance = string.Empty;
        float mValidity = 0.7f;

        public SpeechCommandRecogniser(Grammar grammar)
        {
            initRecogniser(grammar);
        }

        void initRecogniser(Grammar grammar)
        {
            mRecogniser = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));
            mRecogniser.SetInputToDefaultAudioDevice();
            mRecogniser.LoadGrammar(grammar);
        }

        public void SetValidity(float validity)
        {
            if (validity > 1.0f)
                validity = 0.0f;
            else if
                (validity < 0.0f)
                validity = 0.0f;

            mValidity = validity;
        }

        public string WaitFor()
        {
            mRecogniser.SpeechRecognized += this.SpeechRecognizedHandler;
            while (mUtterance == string.Empty)
            {
                mRecogniser.Recognize();
            }
            mRecogniser.SpeechRecognized -= this.SpeechRecognizedHandler;

            return mUtterance;
        }


        public void ClearUtterance()
        {
            mUtterance = string.Empty;
        }
        void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs args)
        {
            float derp = args.Result.Confidence;
            if (args.Result.Confidence >= mValidity)
                mUtterance = args.Result.Text;
        }
    }
}
