﻿using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace Pandemic_AI_Framework
{
    [Serializable]
    public class PA_BuildResearchStation_OperationsExpert : 
        PD_Action, 
        IEquatable<PA_BuildResearchStation_OperationsExpert>,
        I_Player_Action
    {
        public int Player { get; private set; }
        public int Build_RS_On { get; private set; }

        #region constructors
        /// <summary>
        /// Normal & Json constructor
        /// </summary>
        /// <param name="player"></param>
        /// <param name="used_CityCard"></param>
        /// <param name="build_RS_On"></param>
        [JsonConstructor]
        public PA_BuildResearchStation_OperationsExpert(
            int player,
            int build_RS_On
            )
        {
            this.Player = player;
            this.Build_RS_On = build_RS_On;
        }

        /// <summary>
        /// private constructor, for custom deep copy purposes only
        /// </summary>
        /// <param name="actionToCopy"></param>
        private PA_BuildResearchStation_OperationsExpert(
            PA_BuildResearchStation_OperationsExpert actionToCopy
            )
        {
            this.Player = actionToCopy.Player;
            this.Build_RS_On = actionToCopy.Build_RS_On;
        }

        public override PD_Action GetCustomDeepCopy()
        {
            return new PA_BuildResearchStation_OperationsExpert(this);
        }
        #endregion

        public override void Execute(
            Random randomness_provider,
            PD_Game game
            )
        {
#if DEBUG
            if (game.GQ_IsInState_ApplyingMainPlayerActions() == false)
            {
                throw new System.Exception("wrong state!");
            }
            else if (Player != game.GQ_CurrentPlayer())
            {
                throw new System.Exception("wrong player...");
            }
            else if (Build_RS_On != game.GQ_CurrentPlayer_Location())
            {
                throw new System.Exception("selected city does not match current player position");
            }
            else if (game.GQ_CurrentPlayer_Role() != PD_Player_Roles.Operations_Expert)
            {
                throw new System.Exception("wrong player role!");
            }
#endif
            game.GO_Place_ResearchStation_OnCity(Build_RS_On);
        }

        public override string GetDescription()
        {
            return String.Format(
                "{0}: BUILD_RS_OPERATIONS_EXPERT on {1}",
                Player.ToString(),
                Build_RS_On.ToString()
                );
        }

        #region equality overrides
        public bool Equals(PA_BuildResearchStation_OperationsExpert other)
        {
            if (this.Player != other.Player)
            {
                return false;
            }
            if (this.Build_RS_On != other.Build_RS_On)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public override bool Equals(PD_Action other)
        {
            if (other is PA_BuildResearchStation_OperationsExpert other_action)
            {
                return Equals(other_action);
            }
            else {
                return false;
            }
        }
        public override bool Equals(object otherObject)
        {
            if (otherObject is PA_BuildResearchStation_OperationsExpert other_action)
            {
                return Equals(other_action);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 31 + Player;
            hash = hash * 31 + Build_RS_On;

            return hash;
        }
        #endregion
    }
}