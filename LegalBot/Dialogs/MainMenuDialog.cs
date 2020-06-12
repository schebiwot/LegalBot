using LegalBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.IO;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;
using LegalBot.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AdaptiveCards;
using Microsoft.Bot.Schema;

namespace LegalBot.Dialogs
{
    public class MainMenuDialog:ComponentDialog
    {
        private readonly BotStateService _botStateService;
        public MainMenuDialog(string dialogId,BotStateService botStateService):base(dialogId)
        {
            _botStateService  = botStateService ?? throw new ArgumentException(nameof(botStateService));
            IntializeWaterfallDialog();
        }
        private void IntializeWaterfallDialog(){
            var waterfallSteps = new WaterfallStep[]
            {
                StartMenuAsync,
                ChooseActionAsync,  
                ThankYouActionAsync,
                
                
            };

            AddDialog(new WaterfallDialog($"{nameof(MainMenuDialog)}.mainFlow", waterfallSteps));
           
            AddDialog(new ChoicePrompt($"{nameof(MainMenuDialog)}.chooseMenu"));
            AddDialog(new ChoicePrompt($"{nameof(MainMenuDialog)}.chooseAction"));
           
            
           

            // set the starting dialog
            InitialDialogId = $"{nameof(MainMenuDialog)}.mainFlow";

        }
       
        private async Task<DialogTurnResult> StartMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {  
            UserDetails userDetails = await _botStateService.UserDetailsAccessor.GetAsync(stepContext.Context, ()=> new UserDetails());
            
           
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text($"Hello {userDetails.FullName} ,Welcome back, Please choose our services from the list\n"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "INFORMATION", "NEWS", " REFFERAL", "SURVEY", "UPDATE", "SHARE","EXIT" }),

                };
                return await stepContext.PromptAsync($"{nameof(MainMenuDialog)}.chooseMenu", promptOptions, cancellationToken);

            

        }


        private async Task<DialogTurnResult> ChooseActionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["chooseMenu"] = ((FoundChoice)stepContext.Result).Value;

            if ((string)stepContext.Values["chooseMenu"] == "SHARE")
            {
                
                var card   = new HeroCard
                 {
                    Title ="Legal Bot",                   
                    Text = "Please forward this link to share this service, to access the chatbot please click the link below",
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Telegram Link", value: "https://telegram.me/legal23bot"), new CardAction(ActionTypes.ImBack, "Go Back", value: "Go Back"),}  
                   
                };

                

               
                var promptOptions = new PromptOptions{
                    Prompt = (Activity)MessageFactory.Attachment(card.ToAttachment()),

                    Choices = ChoiceFactory.ToChoices(new List<string> {"GO BACK","EXIT"})
                };
                

                return await stepContext.PromptAsync($"{nameof(MainMenuDialog)}.chooseAction", promptOptions, cancellationToken);
            }


            else
            {
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Next Option")
                };

                return await stepContext.PromptAsync($"{nameof(MainMenuDialog)}.chooseAction", promptOptions, cancellationToken);
              
            }
           
        } 
         private async Task<DialogTurnResult> ThankYouActionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["chooseAction"] = ((FoundChoice)stepContext.Result).Value;

            UserDetails userDetails = await _botStateService.UserDetailsAccessor.GetAsync(stepContext.Context, ()=> new UserDetails());

             if((string) stepContext.Values["chooseAction"] == "GO BACK")
             {
                string bye = $"Thank you {userDetails.FullName} please choose from any options in the main menu ";
                await stepContext.Context.SendActivityAsync(bye);
               
                

                return await stepContext.BeginDialogAsync($"{nameof(MainMenuDialog)}.chooseMenu",null, cancellationToken);

             }
             else
             {
                string bye = $"GoodBye {userDetails.FullName} ";
                await stepContext.Context.SendActivityAsync(bye);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

             }
                
                
            


            
           
        }
      

       






    }
}