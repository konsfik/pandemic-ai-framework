﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace Pandemic_AI_Framework
{
    [Serializable]
    public class PD_Game : ICustomDeepCopyable<PD_Game>
    {
        #region properties
        public long unique_id;
        public DateTime start_time;
        public DateTime end_time;

        public PD_GameSettings game_settings;
        public PD_GameFSM game_FSM;

        // STATE - RELATED
        public PD_GameStateCounter game_state_counter;

        public List<int> players;
        public Dictionary<int, int> role__per__player;

        public PD_Map map;


        // CONTAINERS
        public PD_GameCards cards;
        public PD_MapElements map_elements;

        // GAME HISTORY
        public List<PD_Action> PlayerActionsHistory;
        public List<PD_InfectionReport> InfectionReports;

        public List<PD_Action> CurrentAvailablePlayerActions;
        public List<PD_MacroAction> CurrentAvailableMacros;

        #endregion

        #region constructors
        /// <summary>
        /// Creates a game of specific number of players and difficulty
        /// and automatically performs the game - setup
        /// </summary>
        /// <param name="randomness_provider"></param>
        /// <param name="number_of_players"></param>
        /// <param name="game_difficulty"></param>
        /// <returns></returns>
        public static PD_Game Create_Game__RandomRoles(
            Random randomness_provider,
            int number_of_players,
            int game_difficulty
            )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new game with the following settings:
        /// - number of players
        /// - game difficulty
        /// - specific role per player
        /// and performs the game setup, generaiting a randomized initial state
        /// </summary>
        /// <param name="randomness_provider"></param>
        /// <param name="number_of_players"></param>
        /// <param name="game_difficulty"></param>
        /// <param name="role__per__player"></param>
        /// <returns></returns>
        public static PD_Game Create_Game__SpecificRolePerPlayer(
            Random randomness_provider,
            int number_of_players,
            int game_difficulty,
            Dictionary<int, int> role__per__player
            )
        {
            List<int> players = new List<int>();
            for (int i = 0; i < number_of_players; i++)
            {
                players.Add(i);
            }

            List<int> cities = PD_Game_Data_Assistant.Default__Cities();
            Dictionary<int, List<int>> neighbors__per__city = PD_Game_Data_Assistant.Default__Neighbors__Per__City();
            Dictionary<int, int> infection_type__per__city = PD_Game_Data_Assistant.Default__InfectionType__Per__City();

            // cards...
            List<int> allCityCards = PD_Game_Data_Assistant.Default__CityCards();
            List<int> all_infection_cards = PD_Game_Data_Assistant.Default__InfectionCards();
            List<int> all_epidemic_cards = PD_Game_Data_Assistant.Default__EpidemicCards();

            return new PD_Game(
                randomness_provider,

                number_of_players,
                game_difficulty,
                players,
                role__per__player,

                cities,
                infection_type__per__city,
                neighbors__per__city,

                allCityCards,
                all_infection_cards,
                all_epidemic_cards
                );
        }

        /// <summary>
        /// Creates a new game with the following settings:
        /// - number of players
        /// - game difficulty
        /// - available roles list (the roles will be assigned at random, from this list, to the players)
        /// and performs the initial game setup
        /// </summary>
        /// <param name="randomness_provider"></param>
        /// <param name="number_of_players"></param>
        /// <param name="game_difficulty"></param>
        /// <param name="available_roles_list"></param>
        /// <returns></returns>
        public static PD_Game Create_Game__AvailableRolesList(
            Random randomness_provider,
            int number_of_players,
            int game_difficulty,
            List<int> available_roles_list
            )
        {
            List<int> players = new List<int>();
            for (int i = 0; i < number_of_players; i++)
            {
                players.Add(i);
            }

            List<int> cities = PD_Game_Data_Assistant.Default__Cities();
            Dictionary<int, List<int>> neighbors__per__city = PD_Game_Data_Assistant.Default__Neighbors__Per__City();
            Dictionary<int, int> infection_type__per__city = PD_Game_Data_Assistant.Default__InfectionType__Per__City();

            // cards...
            List<int> allCityCards = PD_Game_Data_Assistant.Default__CityCards();
            List<int> all_infection_cards = PD_Game_Data_Assistant.Default__InfectionCards();
            List<int> all_epidemic_cards = PD_Game_Data_Assistant.Default__EpidemicCards();

            List<int> temp_roles_list = available_roles_list.CustomDeepCopy();
            Dictionary<int, int> role__per__player = new Dictionary<int, int>();
            foreach (int player in players)
            {
                int role = temp_roles_list.DrawOneRandom(randomness_provider);
                role__per__player.Add(player, role);
            }

            return new PD_Game(
                randomness_provider,

                number_of_players,
                game_difficulty,
                players,
                role__per__player,

                cities,
                infection_type__per__city,
                neighbors__per__city,

                allCityCards,
                all_infection_cards,
                all_epidemic_cards
                );
        }

        /// <summary>
        /// Creates a game with the following settings:
        /// - game difficulty
        /// the number of players is set to 4, and the player roles are specific:
        /// - operations expert, 
        /// - researcher
        /// - medic
        /// - scientist
        /// </summary>
        /// <param name="randomness_provider"></param>
        /// <param name="game_difficulty"></param>
        /// <returns></returns>
        public static PD_Game Create_Default_Testing(
            Random randomness_provider,
            int game_difficulty
            )
        {

            List<int> players = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                players.Add(i);
            }

            List<int> cities = PD_Game_Data_Assistant.Default__Cities();
            Dictionary<int, List<int>> neighbors__per__city = PD_Game_Data_Assistant.Default__Neighbors__Per__City();
            Dictionary<int, int> infection_type__per__city = PD_Game_Data_Assistant.Default__InfectionType__Per__City();

            // cards...
            List<int> allCityCards = PD_Game_Data_Assistant.Default__CityCards();
            List<int> all_infection_cards = PD_Game_Data_Assistant.Default__InfectionCards();
            List<int> all_epidemic_cards = PD_Game_Data_Assistant.Default__EpidemicCards();


            Dictionary<int, int> role__per__player = new Dictionary<int, int>() {
                {0, PD_Player_Roles.Operations_Expert},
                {1, PD_Player_Roles.Researcher},
                {2, PD_Player_Roles.Medic},
                {3, PD_Player_Roles.Scientist}
            };

            return new PD_Game(
                randomness_provider,

                players.Count,
                game_difficulty,
                players,
                role__per__player,

                cities,
                infection_type__per__city,
                neighbors__per__city,

                allCityCards,
                all_infection_cards,
                all_epidemic_cards
                );
        }

        // normal constructor
        private PD_Game(
            Random randomness_provider,
            int number_of_players,
            int level_of_difficulty,
            List<int> assigned__players,
            Dictionary<int, int> assigned__role__per__player,

            // map - related
            List<int> cities,
            Dictionary<int, int> infection_type__per__city,
            Dictionary<int, List<int>> neighbors_per_city,

            List<int> initial_container__city_cards,
            List<int> initial_container__infection_cards,
            List<int> initial_container__epidemic_cards
            )
        {
            if (assigned__players.Count != number_of_players)
            {
                throw new Exception("number of players is wrong");
            }
            else if (assigned__role__per__player.Keys.Count != number_of_players)
            {
                throw new Exception("number of roles not correct");
            }

            this.players = assigned__players.CustomDeepCopy();
            this.role__per__player = assigned__role__per__player.CustomDeepCopy();

            // initialize parts...
            this.start_time = DateTime.UtcNow;
            this.unique_id = DateTime.UtcNow.Ticks;
            this.unique_id = this.start_time.Ticks;

            CurrentAvailablePlayerActions = new List<PD_Action>();
            CurrentAvailableMacros = new List<PD_MacroAction>();
            PlayerActionsHistory = new List<PD_Action>();
            InfectionReports = new List<PD_InfectionReport>();

            this.game_settings = new PD_GameSettings(level_of_difficulty);

            this.game_state_counter = new PD_GameStateCounter(number_of_players);

            this.game_FSM = new PD_GameFSM(this);

            this.map = new PD_Map(
                cities.Count,
                cities,
                infection_type__per__city,
                neighbors_per_city);

            this.map_elements = new PD_MapElements(this.players, cities);
            this.cards = new PD_GameCards(this.players);



            //////////////////////////////////////////////////////////////////////
            /// PERFORM THE GAME SETUP, HERE!
            //////////////////////////////////////////////////////////////////////

            // 1. Set out board and pieces


            // 1.1. put research stations in research stations container
            map_elements.available_research_stations = 6;

            // 1.2. separate the infection cubes by color (type) in their containers
            for (int i = 0; i < 4; i++)
            {
                map_elements.available_infection_cubes__per__type[i] = 24;
            }

            // 1.3. place research station on atlanta
            int atlanta = 0;
            PD_Game_Operators.GO_Place_ResearchStation_OnCity(
                this, atlanta);

            // 2. Place outbreaks and cure markers
            // put outbreaks counter to position zero
            game_state_counter.ResetOutbreaksCounter();

            // place cure markers vial side up
            game_state_counter.InitializeCureMarkerStates();

            // 3. place infection marker and infect 9 cities
            // 3.1. place the infection marker (epidemics counter) on the lowest position
            game_state_counter.ResetEpidemicsCounter();

            // 3.2. Infect the first cities - process
            // 3.2.1. put all infection cards in the divided deck of infection cards...
            cards.divided_deck_of_infection_cards.Add(initial_container__infection_cards.DrawAll());

            // 3.2.2 shuffle the infection cards deck...
            cards.divided_deck_of_infection_cards.ShuffleAllSubListsElements(randomness_provider);

            // 3.2.3 actually infect the cities.. 
            var firstPlayer = assigned__players[0];
            for (int num_InfectionCubes_ToPlace = 3; num_InfectionCubes_ToPlace > 0; num_InfectionCubes_ToPlace--)
            {
                for (int city_Counter = 0; city_Counter < 3; city_Counter++)
                {
                    var infectionCard = cards.divided_deck_of_infection_cards.DrawLastElementOfLastSubList();

                    int city = infectionCard;
                    int city_type = map.infection_type__per__city[city];

                    PD_InfectionReport report = new PD_InfectionReport(
                        true,
                        firstPlayer,
                        city,
                        city_type,
                        num_InfectionCubes_ToPlace
                        );

                    PD_InfectionReport finalReport = PD_Game_Operators.GO_InfectCity(
                        this,
                        city,
                        num_InfectionCubes_ToPlace,
                        report,
                        true
                        );

                    InfectionReports.Add(finalReport);

                    cards.deck_of_discarded_infection_cards.Add(infectionCard);
                }
            }

            // 4. Give each player cards and a pawn
            // 4.1. Assign roles (and pawns)
            // -> already done ^^

            // 4.2. Deal cards to players: initial hands
            cards.divided_deck_of_player_cards.Add(initial_container__city_cards.DrawAll());
            cards.divided_deck_of_player_cards.ShuffleAllSubListsElements(randomness_provider);

            int numPlayers = assigned__players.Count;
            int numCardsToDealPerPlayer = game_settings.GetNumberOfInitialCardsToDealPlayers(numPlayers);
            foreach (var player in assigned__players)
            {
                for (int i = 0; i < numCardsToDealPerPlayer; i++)
                {
                    cards.player_hand__per__player[player].Add(cards.divided_deck_of_player_cards.DrawLastElementOfLastSubList());
                }
            }

            // 5. Prepare the player deck
            // 5.1. get the necessary number of epidemic cards
            int numEpidemicCards = game_settings.GetNumberOfEpidemicCardsToUseInGame();

            // divide the player cards deck in as many sub decks as necessary
            var allPlayerCardsList = cards.divided_deck_of_player_cards.DrawAllElementsOfAllSubListsAsOneList();
            //int numberOfPlayerCardsPerSubDeck = allPlayerCardsList.Count / numEpidemicCards;

            int numCards = allPlayerCardsList.Count;
            int numSubDecks = numEpidemicCards;
            int numCardsPerSubDeck = numCards / numSubDecks;
            int remainingCardsNumber = numCards % numSubDecks;

            // create the sub decks
            List<List<int>> temporaryDividedList = new List<List<int>>();
            for (int i = 0; i < numEpidemicCards; i++)
            {
                var subDeck = new List<int>();
                for (int j = 0; j < numCardsPerSubDeck; j++)
                {
                    subDeck.Add(allPlayerCardsList.DrawOneRandom(randomness_provider));
                }
                temporaryDividedList.Add(subDeck);
            }
            // add the remaining cards
            for (int i = 0; i < remainingCardsNumber; i++)
            {
                int deckIndex = (temporaryDividedList.Count - 1) - i;
                temporaryDividedList[deckIndex].Add(allPlayerCardsList.DrawOneRandom(randomness_provider));
            }
            // insert the epidemic cards
            foreach (List<int> subList in temporaryDividedList)
            {
                int epidemic_card = initial_container__epidemic_cards.DrawFirst();
                subList.Add(epidemic_card);
            }

            // shuffle all the sublists!
            temporaryDividedList.ShuffleAllSubListsElements(randomness_provider);

            // set the player cards deck as necessary
            cards.divided_deck_of_player_cards.Clear();
            foreach (var sublist in temporaryDividedList)
            {
                cards.divided_deck_of_player_cards.Add(sublist);
            }

            //// place all pawns on atlanta
            PD_Game_Operators.GO_PlaceAllPawnsOnAtlanta(this);


            UpdateAvailablePlayerActions();
        }


        [JsonConstructor]
        public PD_Game(
            long unique_id,
            DateTime start_time,
            DateTime end_time,

            PD_GameSettings game_settings,
            PD_GameFSM game_FSM,
            PD_GameStateCounter game_state_counter,

            List<int> players,
            PD_Map map,

            PD_MapElements map_elements,
            PD_GameCards cards,

            Dictionary<int, int> role__per__player,

            List<PD_Action> playerActionsHistory,
            List<PD_InfectionReport> infectionReports,

            List<PD_Action> currentAvailablePlayerActions,
            List<PD_MacroAction> currentAvailableMacros
            )
        {
            this.unique_id = unique_id;
            this.start_time = start_time;
            this.end_time = end_time;

            this.game_settings = game_settings.GetCustomDeepCopy();
            this.game_FSM = game_FSM.GetCustomDeepCopy();
            this.game_state_counter = game_state_counter.GetCustomDeepCopy();

            this.players = players.CustomDeepCopy();
            this.map = map.GetCustomDeepCopy();

            this.map_elements = map_elements.GetCustomDeepCopy();
            this.cards = cards.GetCustomDeepCopy();

            this.role__per__player = role__per__player.CustomDeepCopy();

            this.PlayerActionsHistory = playerActionsHistory.CustomDeepCopy();
            this.InfectionReports = infectionReports.CustomDeepCopy();

            this.CurrentAvailablePlayerActions = currentAvailablePlayerActions.CustomDeepCopy();
            this.CurrentAvailableMacros = currentAvailableMacros.CustomDeepCopy();
        }

        /// <summary>
        /// private constructor, for deep copy purposes, only!
        /// </summary>
        /// <param name="gameToCopy"></param>
        private PD_Game(
            PD_Game gameToCopy
            )
        {
            unique_id = gameToCopy.unique_id;
            start_time = gameToCopy.start_time;
            end_time = gameToCopy.end_time;

            game_settings = gameToCopy.game_settings.GetCustomDeepCopy();
            game_FSM = gameToCopy.game_FSM.GetCustomDeepCopy();
            game_state_counter = gameToCopy.game_state_counter.GetCustomDeepCopy();

            players = gameToCopy.players.CustomDeepCopy();
            map = gameToCopy.map.GetCustomDeepCopy();

            map_elements = gameToCopy.map_elements.GetCustomDeepCopy();
            cards = gameToCopy.cards.GetCustomDeepCopy();

            role__per__player = gameToCopy.role__per__player.CustomDeepCopy();

            PlayerActionsHistory = gameToCopy.PlayerActionsHistory.CustomDeepCopy();
            InfectionReports = gameToCopy.InfectionReports.CustomDeepCopy();

            CurrentAvailablePlayerActions = gameToCopy.CurrentAvailablePlayerActions.CustomDeepCopy();
            CurrentAvailableMacros = gameToCopy.CurrentAvailableMacros.CustomDeepCopy();
        }

        #endregion

        #region command methods
        public void Medic_MoveTreat(
            int city
            )
        {
            // if player is a medic:
            if (this.GQ_CurrentPlayer_Role() == PD_Player_Roles.Medic)
            {
                List<int> typesOfInfectionCubesOnTargetLocation = this.GQ_InfectionCubeTypes_OnCity(
                    city
                    );
                foreach (var type in typesOfInfectionCubesOnTargetLocation)
                {
                    // if this type has been cured:
                    if (this.GQ_Is_DiseaseCured_OR_Eradicated(type))
                    {
                        PD_Game_Operators.GO_Remove_All_InfectionCubes_OfType_FromCity(
                            this,
                            city,
                            type
                            );
                        // create the supposed (auto) action
                        PA_TreatDisease_Medic_Auto actionToStore = new PA_TreatDisease_Medic_Auto(
                            this.GQ_CurrentPlayer(),
                            city,
                            type
                            );

                        // store the action in the game actions history
                        PlayerActionsHistory.Add(actionToStore);
                    }

                }
            }
        }

        public void Com_PA_TreatDisease_Medic(
            int player,
            int city,
            int treat_Type
            )
        {
            // remove all cubes of this type
            PD_Game_Operators.GO_Remove_All_InfectionCubes_OfType_FromCity(
                this,
                city,
                treat_Type
                );

            if (this.GQ_Is_DiseaseCured_OR_Eradicated(treat_Type))
            {
                

                // check if disease is eradicated...
                int remaining_cubes_this_type
                    = map_elements.available_infection_cubes__per__type[treat_Type];

                // if disease eradicated -> set marker to 2
                if (remaining_cubes_this_type == 0)
                {
                    game_state_counter.disease_states[treat_Type] = 2;
                }
            }
        }

        public void Medic_AutoTreat_AfterDiscoverCure(int curedDiseaaseType)
        {
            int medicLocation = this.GQ_Medic_Location();
            if (medicLocation != -1)
            {
                List<int> infectionCubeTypes_OnMedicLocation =
                    this.GQ_InfectionCubeTypes_OnCity(medicLocation);

                if (infectionCubeTypes_OnMedicLocation.Contains(curedDiseaaseType))
                {
                    // remove all cubes of this type from medic's location
                    PD_Game_Operators.GO_Remove_All_InfectionCubes_OfType_FromCity(
                        this,
                        medicLocation,
                        curedDiseaaseType
                        );
                }
            }
        }
        #endregion

        public void UpdateAvailablePlayerActions()
        {
            CurrentAvailablePlayerActions = this.FindAvailable_PlayerActions();
        }

        public void Apply_Action(
            Random randomness_provider,
            PD_Action playerAction
            )
        {
            CurrentAvailableMacros = new List<PD_MacroAction>();

            if (CurrentAvailablePlayerActions.Contains(playerAction) == false)
            {
                throw new System.Exception("non applicable action");
            }

            PlayerActionsHistory.Add(playerAction);

            game_FSM.OnCommand(randomness_provider, this, playerAction);

            UpdateAvailablePlayerActions();

            // after an action is applied, see if the next action is auto action
            // and if so, apply it automatically.
            if (
                CurrentAvailablePlayerActions != null
                && CurrentAvailablePlayerActions.Count > 0
                )
            {
                if (CurrentAvailablePlayerActions[0] is I_Auto_Action)
                {
                    Apply_Action(
                        randomness_provider,
                        CurrentAvailablePlayerActions[0]
                        );
                }
            }
        }

        public List<PD_MacroAction> GetAvailableMacros(PD_AI_PathFinder pathFinder)
        {
            if (
                CurrentAvailableMacros == null
                ||
                CurrentAvailableMacros.Count == 0
                )
            {
                var researchStationCities = this.GQ_ResearchStationCities();
                CurrentAvailableMacros = PD_MacroActionsSynthesisSystem.FindAll_Macros(
                    this,
                    pathFinder,
                    researchStationCities
                    );
                return new List<PD_MacroAction>(CurrentAvailableMacros);
            }

            return new List<PD_MacroAction>(CurrentAvailableMacros);
        }

        public void Apply_Macro_Action(
            Random randomness_provider,
            PD_MacroAction macro
            )
        {
            if (game_FSM.CurrentState.GetType() == typeof(PD_GS_ApplyingMainPlayerActions))
            {
                if (
                    macro.MacroAction_Type == PD_MacroAction_Type.Walk_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.Stay_Macro

                    || macro.MacroAction_Type == PD_MacroAction_Type.TreatDisease_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.TreatDisease_Medic_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.AutoTreatDisease_Medic_Macro

                    || macro.MacroAction_Type == PD_MacroAction_Type.BuildResearchStation_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.BuildResearchStation_OperationsExpert_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.MoveResearchStation_Macro

                    || macro.MacroAction_Type == PD_MacroAction_Type.ShareKnowledge_Give_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.ShareKnowledge_Give_ResearcherGives_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.ShareKnowledge_Take_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.ShareKnowledge_Take_FromResearcher_Macro

                    || macro.MacroAction_Type == PD_MacroAction_Type.TakePositionFor_ShareKnowledge_Give_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.TakePositionFor_ShareKnowledge_Take_Macro

                    || macro.MacroAction_Type == PD_MacroAction_Type.DiscoverCure_Macro
                    || macro.MacroAction_Type == PD_MacroAction_Type.DiscoverCure_Scientist_Macro
                    )
                {
                    var executablePart = new List<PD_Action>();
                    int numRemainingActionsThisRound = 4 - game_state_counter.player_action_index;
                    for (int i = 0; i < numRemainingActionsThisRound; i++)
                    {
                        if (i < macro.Count_Total_Length())
                        {
                            executablePart.Add(macro.Actions_All[i].GetCustomDeepCopy());
                        }
                    }

                    foreach (var command in executablePart)
                    {
                        if (CurrentAvailablePlayerActions.Contains(command))
                        {
                            Apply_Action(
                                randomness_provider,
                                command
                                );
                        }
                        else
                        {
                            Console.WriteLine(macro.GetDescription());
                            Console.WriteLine("problem: " + command.GetDescription());
                            throw new System.Exception("command cannot be applied!");
                        }
                    }
                }
                else
                {
                    throw new System.Exception("wrong macro type");
                }
            }
            else if (this.GQ_IsInState_DiscardAfterDrawing())
            {
                PD_MacroAction_Type mat = macro.MacroAction_Type;
                if (mat == PD_MacroAction_Type.Discard_AfterDrawing_Macro)
                {
                    var action = macro.Actions_All[0];
                    if (CurrentAvailablePlayerActions.Contains(action))
                    {
                        //Console.WriteLine("applying command: " + action.GetDescription());
                        Apply_Action(
                            randomness_provider,
                            action
                            );
                    }
                    else
                    {
                        throw new System.Exception("command cannot be applied!");
                    }
                }
                else
                {
                    throw new System.Exception("wrong macro type");
                }
            }
            else if (this.GQ_IsInState_DiscardDuringMainPlayerActions())
            {
                PD_MacroAction_Type mat = macro.MacroAction_Type;
                if (mat == PD_MacroAction_Type.Discard_DuringMainPlayerActions_Macro)
                {
                    var action = macro.Actions_All[0];
                    if (CurrentAvailablePlayerActions.Contains(action))
                    {
                        //Console.WriteLine("applying command: " + action.GetDescription());
                        Apply_Action(
                            randomness_provider,
                            action
                            );
                    }
                    else
                    {
                        throw new System.Exception("command cannot be applied!");
                    }
                }
                else
                {
                    throw new System.Exception("wrong macro type");
                }
            }
        }

        public void OverrideStartTime()
        {
            start_time = DateTime.UtcNow;
        }

        public void OverrideStartTime(DateTime date_time)
        {
            start_time = date_time;
        }

        /// <summary>
        /// This method is called when entering the GameLost or the GameWon states
        /// </summary>
        public void OverrideEndTime()
        {
            end_time = DateTime.UtcNow;
        }

        public void OverrideEndTime(DateTime date_time)
        {
            end_time = date_time;
        }

        public PD_Game Request_Randomized_Copy(Random randomness_provider)
        {
            PD_Game gameCopy = this.GetCustomDeepCopy();
            gameCopy.GO_Randomize_HiddenState(randomness_provider);
            return gameCopy;
        }

        public PD_Game GetCustomDeepCopy()
        {
            return new PD_Game(this);
        }

        #region equality overrides
        public bool Equals(PD_Game other)
        {
            if (this.unique_id != other.unique_id)
            {
                return false;
            }
            else if (this.start_time != other.start_time)
            {
                return false;
            }
            else if (this.end_time != other.end_time)
            {
                return false;
            }
            else if (this.game_settings.Equals(other.game_settings) == false)
            {
                return false;
            }
            else if (this.game_FSM.Equals(other.game_FSM) == false)
            {
                return false;
            }
            else if (this.game_state_counter.Equals(other.game_state_counter) == false)
            {
                return false;
            }
            else if (this.players.List_Equals(other.players) == false)
            {
                return false;
            }
            else if (this.map.Equals(other.map) == false)
            {
                return false;
            }
            else if (this.cards.Equals(other.cards) == false)
            {
                return false;
            }
            else if (this.map_elements.Equals(other.map_elements) == false)
            {
                return false;
            }
            else if (this.role__per__player.Dictionary_Equals(other.role__per__player) == false)
            {
                return false;
            }
            else if (this.PlayerActionsHistory.List_Equals(other.PlayerActionsHistory) == false)
            {
                return false;
            }
            else if (this.InfectionReports.List_Equals(other.InfectionReports) == false)
            {
                return false;
            }
            else if (this.CurrentAvailablePlayerActions.List_Equals(other.CurrentAvailablePlayerActions) == false)
            {
                return false;
            }
            else if (this.CurrentAvailableMacros.List_Equals(other.CurrentAvailableMacros) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool Equals(object otherObject)
        {
            if (otherObject is PD_Game other_game)
            {
                return Equals(other_game);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 31 + unique_id.GetHashCode();
            hash = hash * 31 + start_time.GetHashCode();
            hash = hash * 31 + end_time.GetHashCode();

            hash = hash * 31 + game_settings.GetHashCode();
            hash = hash * 31 + game_FSM.GetHashCode();

            hash = hash * 31 + game_state_counter.GetHashCode();

            hash = hash * 31 + players.Custom_HashCode();
            hash = hash * 31 + map.GetHashCode();

            hash = hash * 31 + cards.GetHashCode();
            hash = hash * 31 + map_elements.GetHashCode();

            hash = hash * 31 + role__per__player.Custom_HashCode();

            hash = hash * 31 + PlayerActionsHistory.Custom_HashCode();
            hash = hash * 31 + InfectionReports.Custom_HashCode();

            hash = hash * 31 + CurrentAvailablePlayerActions.Custom_HashCode();
            hash = hash * 31 + CurrentAvailableMacros.Custom_HashCode();

            return hash;
        }

        public static bool operator ==(PD_Game c1, PD_Game c2)
        {
            if (Object.ReferenceEquals(c1, null))
            {
                if (Object.ReferenceEquals(c2, null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else // c1 is not null
            {
                if (Object.ReferenceEquals(c2, null)) // c2 is null
                {
                    return false;
                }
            }
            // c1 is not null && c2 is not null
            // -> actually check equality
            return c1.Equals(c2);
        }

        public static bool operator !=(PD_Game c1, PD_Game c2)
        {
            return !(c1 == c2);
        }

        #endregion
    }
}
