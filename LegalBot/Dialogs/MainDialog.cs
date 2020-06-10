using LegalBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegalBot.Models;


namespace LegalBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        public MainDialog(BotStateService botStateService)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));
            InitializeWaterfallDialog();
        }
        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialSetupAsync,
                FinalStepAsync
            };
            // which dialog we are going to call...

            AddDialog(new EnglishDetailsDialog($"{nameof(MainDialog)}.englishDialog", _botStateService));
            AddDialog(new MainMenuDialog($"{nameof(MainDialog)}.mainMenuDialog", _botStateService));
            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));

            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
        }
        private async Task<DialogTurnResult> InitialSetupAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {   
            UserDetails userDetails = await _botStateService.UserDetailsAccessor.GetAsync(stepContext.Context, ()=> new UserDetails());

            if(string.IsNullOrEmpty(userDetails.FullName)){
                
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.englishDialog", null, cancellationToken);
               
            }
            else{
           
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.mainMenuDialog", null, cancellationToken);
            }
            
             
        }
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }

}
