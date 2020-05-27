using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegalBot.Models;
using Microsoft.Bot.Builder.Dialogs;

namespace LegalBot.Services
{
    
    public class BotStateService
    {
        // State Variables
        public UserState UserState { get; }
        public ConversationState ConversationState { get; }
        // IDs is nameof Botstateservice
        // For identifying 
        public static string ConversationDataId { get; } = $"{nameof(BotStateService)}.ConversationData";
        public static string UserDetailsId{ get; } = $"{nameof(BotStateService)}.UserDetails";
        public static string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";
        // Accessor 
        // Helps to access data inside state management bucket by pull,pushing and deleting data 
        public IStatePropertyAccessor<UserDetails> UserDetailsAccessor { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
        // We inject conversation state..
        // Constructor 
        public BotStateService(ConversationState conversationState, UserState userState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            intializeAccessors();
        }


        public void intializeAccessors()
        {
            // Intialize Converstioin State Accessors
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
            // intialize User state the properties we want to store using the IDs
            UserDetailsAccessor = UserState.CreateProperty<UserDetails>(UserDetailsId);
            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
        }
    }
}

