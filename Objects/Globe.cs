using System.Diagnostics;

namespace GridExplorerBot
{
    public class Globe : StaticObject
    {
        enum State
        {
            EuropeAfrica,
            AsiaAustralia,
            Americas,
        }

        State mState { get => (State)_mState; set => _mState = (int)value; }
        int _mState = 0;

        public override string Render()
        {
            switch (mState)
            {
                case State.EuropeAfrica: return Emoji.Environment.GlobeShowingEuropeAfrica;
                case State.AsiaAustralia: return Emoji.Environment.GlobeShowingAsiaAustralia;
                case State.Americas: return Emoji.Environment.GlobeShowingAmericas;
            }

            Debug.Fail("unexpected state");

            return "";
        }

        public override void Stream(SaveStream stream)
        {
            stream.Stream(ref _mState, 2);
        }

        public override ActionResult UseWithoutTarget(Game game)
        {
            switch (mState)
            {
                case State.EuropeAfrica: mState = State.AsiaAustralia; break;
                case State.AsiaAustralia: mState = State.Americas; break;
                case State.Americas: mState = State.EuropeAfrica; break;
            }

            return new ActionResult()
            {
                mOutput = "You give the globe a good spin and it ends up facing a different direction.",
                mSuccess = true,
            };
        }
    }
}