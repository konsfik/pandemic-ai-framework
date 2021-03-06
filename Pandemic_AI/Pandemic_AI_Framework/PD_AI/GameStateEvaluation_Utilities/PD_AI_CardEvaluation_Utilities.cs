﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandemic_AI_Framework
{
    public static class PD_AI_CardEvaluation_Utilities
    {
        #region num cards table
        // these methods are the very basic ones
        // for calculating all the evaluations.
        // they just count the number of cards that are in the hands of each player.

        /// <summary>
        /// Returns a table that just ccounts the number of cards in the players hands, per player and per type.
        /// The table is a 2D table, where rows are of the same type, and columns are of the same player.
        /// Info can be accessed as: 
        /// numCards = numCardsTable[typeIndex, playerIndex]
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static int[,] NumCardsTable(
            PD_Game game
            )
        {
            int numPlayers = game.players.Count;
            int numTypes = 4;

            int[,] numCardsTable = new int[numTypes, numPlayers];

            for (int playerIndex = 0; playerIndex < numPlayers; playerIndex++)
            {
                var player = game.players[playerIndex];
                var cityCardsInPlayerHand =
                    game.GQ_CityCardsInPlayerHand(
                        player
                        );

                foreach (int city_card in cityCardsInPlayerHand)
                {
                    int city_card_type = game.map.infection_type__per__city[city_card];
                    numCardsTable[city_card_type, playerIndex]++;
                }
            }

            return numCardsTable;
        }


        #endregion

        #region percent complete sets of cards
        public static double[,] Calculate_Percent_CompleteSetsOfCards_Table(
            PD_Game game
            )
        {
            int numPlayers = game.players.Count;
            int numTypes = 4;

            // step 2: calculate percent complete sets of cards table
            double[,] percent_CompleteSetsOfCards_Table = new double[numTypes, numPlayers];

            for (int pi = 0; pi < numPlayers; pi++)
            {
                for (int ti = 0; ti < numTypes; ti++)
                {
                    bool isPlayerScientist = game.GQ_Player_Role(pi) == PD_Player_Roles.Scientist;

                    int num_city_cards_of_type_in_player_hand 
                        = game.GQ_Num_CityCards_OfType_InPlayerHand(pi, ti);

                    int numCards_SetComplete = isPlayerScientist ? 4 : 5;

                    double percent_SetCompleteness = 
                        (double)num_city_cards_of_type_in_player_hand / (double)numCards_SetComplete;
                    if (percent_SetCompleteness > 1.0)
                    {
                        percent_SetCompleteness = 1.0;
                    }

                    percent_CompleteSetsOfCards_Table[ti, pi] = percent_SetCompleteness;
                }
            }

            return percent_CompleteSetsOfCards_Table;
        }

        public static double[,] Calculate_Percent_CompleteSetsOfCards_Table(
            PD_Game game,
            int[,] numCardsTable
            )
        {
            int numTypes = numCardsTable.Height();
            int numPlayers = numCardsTable.Width();

            double[,] percent_CompleteSetsOfCards_Table = new double[numTypes, numPlayers];

            for (int pi = 0; pi < numPlayers; pi++)
            {
                bool isPlayerScientist = game.GQ_Player_Role(pi) == PD_Player_Roles.Scientist;
                for (int ti = 0; ti < numTypes; ti++)
                {
                    int num_cards_this_player_this_type = numCardsTable[ti, pi];
                    int num_cards_complete_set = isPlayerScientist ? 4 : 5;

                    double percent_SetCompleteness = 
                        (double)num_cards_this_player_this_type / (double)num_cards_complete_set;
                    if (percent_SetCompleteness > 1.0)
                    {
                        percent_SetCompleteness = 1.0;
                    }

                    percent_CompleteSetsOfCards_Table[ti, pi] = percent_SetCompleteness;
                }
            }

            return percent_CompleteSetsOfCards_Table;
        }

        #endregion

        #region group ability to cure diseases Table
        public static double[,] Calculate_Percent_AbilityToCureDiseases_Table(
            PD_Game game,
            double[,] percentCompleteSetsOfCards_Table,
            bool squared
            )
        {
            double[,] groupAbilityToCureDiseases_Table = percentCompleteSetsOfCards_Table.CustomDeepCopy();

            int numTypes = groupAbilityToCureDiseases_Table.Height();
            int numPlayers = groupAbilityToCureDiseases_Table.Width();

            for (int type = 0; type < numTypes; type++)
            {
                if (game.GQ_Is_DiseaseCured_OR_Eradicated(type))
                {
                    for (int playerIndex = 0; playerIndex < numPlayers; playerIndex++)
                    {
                        groupAbilityToCureDiseases_Table[type, playerIndex] = 1;
                    }
                }
            }

            if (squared)
            {
                groupAbilityToCureDiseases_Table = groupAbilityToCureDiseases_Table.Squared();
            }

            return groupAbilityToCureDiseases_Table;
        }
        #endregion

        public static double Calculate_PercentCuredDiseases_Gradient(
            PD_Game game,
            bool squared
            )
        {
            int[,] numCardsTable = NumCardsTable(game);

            double[,] percentComplete_Table = Calculate_Percent_CompleteSetsOfCards_Table(game, numCardsTable);

            double[,] groupAbilityToCureDiseases_Table = Calculate_Percent_AbilityToCureDiseases_Table(
                game,
                percentComplete_Table,
                squared
                );

            double[,] finalTable = groupAbilityToCureDiseases_Table.CustomDeepCopy();

            int numTypes = groupAbilityToCureDiseases_Table.Height();
            int numPlayers = groupAbilityToCureDiseases_Table.Width();

            for (int type = 0; type < numTypes; type++)
            {
                bool isDiseaseCured = game.GQ_Is_DiseaseCured_OR_Eradicated(type);
                if (isDiseaseCured)
                {
                    for (int playerIndex = 0; playerIndex < numPlayers; playerIndex++)
                    {
                        finalTable[type, playerIndex] = 1.3;
                    }
                }
                else
                {
                    for (int playerIndex = 0; playerIndex < numPlayers; playerIndex++)
                    {
                        finalTable[type, playerIndex] = groupAbilityToCureDiseases_Table[type, playerIndex];
                    }
                }
            }

            double[,] normalized_Table = finalTable.Divided_By(1.3);

            if (squared)
            {
                normalized_Table = normalized_Table.Squared();
            }

            return
                normalized_Table
                .RowMax_PerRow()
                .Average();
        }

        #region GroupAbilityToCureDiseases
        public static double Calculate_Percent_AbilityToCureDiseases(
            PD_Game game,
            double[,] groupAbilityToCureDiseases_Table
            )
        {
            return
                groupAbilityToCureDiseases_Table
                .RowMax_PerRow()
                .Average();
        }

        public static double Calculate_Percent_AbilityToCureDiseases(
            PD_Game game,
            bool squared
            )
        {
            int[,] numCardsTable = NumCardsTable(
                game
                );
            double[,] percentComplete_SetsOfCards_Table = Calculate_Percent_CompleteSetsOfCards_Table(
                game,
                numCardsTable
                );
            double[,] groupAbilityToCureDiseases_Table = Calculate_Percent_AbilityToCureDiseases_Table(
                game,
                percentComplete_SetsOfCards_Table,
                squared
                );
            return
                groupAbilityToCureDiseases_Table
                .RowMax_PerRow()
                .Average();
        }
        #endregion

        #region effect of player actions
        public static int[,] NumCardsTable_AfterApplying_PlayerAction(
            PD_Game game,
            int[,] initialNumCardsTable,
            PD_Action action
            )
        {
            int[,] hypothetical_NumCardsTable = initialNumCardsTable.CustomDeepCopy();
            if (action is PA_ShareKnowledge_GiveCard give_card_action)
            {
                int cardType = game.GQ_City_InfectionType(give_card_action.CityCardToGive);
                int giver_Index = game.players.IndexOf(give_card_action.Player);
                int taker_Index = game.players.IndexOf(give_card_action.OtherPlayer);

                hypothetical_NumCardsTable[cardType, giver_Index] -= 1;
                hypothetical_NumCardsTable[cardType, taker_Index] += 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_ShareKnowledge_GiveCard_ResearcherGives researcher_gives_action)
            {
                int cardType = game.GQ_City_InfectionType(researcher_gives_action.CityCardToGive);
                int giver_Index = game.players.IndexOf(researcher_gives_action.Player);
                int taker_Index = game.players.IndexOf(researcher_gives_action.OtherPlayer);

                hypothetical_NumCardsTable[cardType, giver_Index] -= 1;
                hypothetical_NumCardsTable[cardType, taker_Index] += 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_ShareKnowledge_TakeCard take_card_action)
            {
                int cardType = game.GQ_City_InfectionType(take_card_action.CityCardToTake);
                int taker_Index = game.players.IndexOf(take_card_action.Player);
                int giver_Index = game.players.IndexOf(take_card_action.OtherPlayer);

                hypothetical_NumCardsTable[cardType, giver_Index] -= 1;
                hypothetical_NumCardsTable[cardType, taker_Index] += 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_ShareKnowledge_TakeCard_FromResearcher take_card_from_researcher_action)
            {
                int cardType = game.GQ_City_InfectionType(take_card_from_researcher_action.CityCardToTake);
                int taker_Index = game.players.IndexOf(take_card_from_researcher_action.Player);
                int giver_Index = game.players.IndexOf(take_card_from_researcher_action.OtherPlayer);

                hypothetical_NumCardsTable[cardType, giver_Index] -= 1;
                hypothetical_NumCardsTable[cardType, taker_Index] += 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_DirectFlight directFlightAction)
            {
                int cardType = game.GQ_City_InfectionType(directFlightAction.UsedCard);
                int playerIndex = game.players.IndexOf(directFlightAction.Player);
                hypothetical_NumCardsTable[cardType, playerIndex] -= 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_CharterFlight charter_flight_action)
            {
                int cardType = game.GQ_City_InfectionType(charter_flight_action.UsedCard);
                int playerIndex = game.players.IndexOf(charter_flight_action.Player);
                hypothetical_NumCardsTable[cardType, playerIndex] -= 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_OperationsExpert_Flight operations_expert_flight_action)
            {
                int cardType = game.GQ_City_InfectionType(operations_expert_flight_action.UsedCard);
                int playerIndex = game.players.IndexOf(operations_expert_flight_action.Player);
                hypothetical_NumCardsTable[cardType, playerIndex] -= 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_BuildResearchStation build_research_station_action)
            {
                int cardType = game.GQ_City_InfectionType(build_research_station_action.UsedCard);
                int playerIndex = game.players.IndexOf(build_research_station_action.Player);
                hypothetical_NumCardsTable[cardType, playerIndex] -= 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_MoveResearchStation move_research_station_action)
            {
                int cardType = game.GQ_City_InfectionType(move_research_station_action.Used_CityCard);
                int playerIndex = game.players.IndexOf(move_research_station_action.Player);
                hypothetical_NumCardsTable[cardType, playerIndex] -= 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_Discard_DuringMainPlayerActions discard_during_action)
            {
                int cardType = game.GQ_City_InfectionType(discard_during_action.PlayerCardToDiscard);
                int playerIndex = game.players.IndexOf(discard_during_action.Player);
                hypothetical_NumCardsTable[cardType, playerIndex] -= 1;

                return hypothetical_NumCardsTable;
            }
            else if (action is PA_Discard_AfterDrawing discard_after_action)
            {
                int cardType = game.GQ_City_InfectionType(discard_after_action.PlayerCardToDiscard);
                int playerIndex = game.players.IndexOf(discard_after_action.Player);
                hypothetical_NumCardsTable[cardType, playerIndex] -= 1;

                return hypothetical_NumCardsTable;
            }
            else
            {
                return hypothetical_NumCardsTable;
            }
        }

        public static int[,] NumCardsTable_AfterApplying_ListOfActions(
            PD_Game game,
            int[,] initialNumCardsTable,
            List<PD_Action> actions
            )
        {
            int[,] result = initialNumCardsTable.CustomDeepCopy();
            foreach (var action in actions)
            {
                result = NumCardsTable_AfterApplying_PlayerAction(
                    game,
                    result,
                    action
                    );
            }
            return result;
        }

        public static double Calculate_PlayerAction_Effect_On_Percent_AbilityToCureDiseases(
            PD_Game game,
            PD_Action action,
            bool squared
            )
        {
            int[,] current_NumCardsTable = NumCardsTable(game);
            double[,] current_Percent_Complete_SetsOfCards = Calculate_Percent_CompleteSetsOfCards_Table(
                game,
                current_NumCardsTable
                );
            double[,] current_GroupAbilityToCureDiseases_Table = Calculate_Percent_AbilityToCureDiseases_Table(
                game,
                current_Percent_Complete_SetsOfCards,
                squared
                );
            double current_GroupAbilityToCureDiseases = Calculate_Percent_AbilityToCureDiseases(
                game,
                current_GroupAbilityToCureDiseases_Table
                );

            int[,] supposed_NumCardsTable = NumCardsTable_AfterApplying_PlayerAction(
                game,
                current_NumCardsTable,
                action
                );
            double[,] supposed_Percent_Complete_SetsOfCards = Calculate_Percent_CompleteSetsOfCards_Table(
                game,
                supposed_NumCardsTable
                );
            double[,] supposed_GroupAbilityToCureDiseases_Table = Calculate_Percent_AbilityToCureDiseases_Table(
                game,
                supposed_Percent_Complete_SetsOfCards,
                squared
                );
            double supposed_GroupAbilityToCureDiseases = Calculate_Percent_AbilityToCureDiseases(
                game,
                supposed_GroupAbilityToCureDiseases_Table
                );

            return supposed_GroupAbilityToCureDiseases - current_GroupAbilityToCureDiseases;
        }

        public static double Calculate_ListOfPlayerActions_Effect_On_Percent_AbilityToCureDiseases(
            PD_Game game,
            List<PD_Action> actions,
            bool squared
            )
        {
            double effect = 0;
            foreach (var action in actions)
            {
                effect += Calculate_PlayerAction_Effect_On_Percent_AbilityToCureDiseases(
                    game,
                    action,
                    squared
                    );
            }
            return effect;
        }
        #endregion
    }
}
