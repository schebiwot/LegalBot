﻿using LegalBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegalBot.Models;


namespace LegalBot.Dialogs
{
    // It inherits from parent class
    public class EnglishDetailsDialog :ComponentDialog
    {
        private readonly BotStateService _botStateService;
        public EnglishDetailsDialog(string dialogId, BotStateService botStateService) : base(dialogId)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));
            InitializeWaterfallDialog();
        }
        // Intialize the waterflow dialog...
        private void InitializeWaterfallDialog()
        {
            // create waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {   SelectedLanguageAsync,
                RegisterNameAsync,
                CountyStepAsync,
                SubCountyStepAsync,
                WardStepAsync,
                ConfirmationStepAsync,
            };

            AddDialog(new WaterfallDialog($"{nameof(EnglishDetailsDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(EnglishDetailsDialog)}.preferredLanguage"));
            AddDialog(new TextPrompt($"{nameof(EnglishDetailsDialog)}.name"));
            AddDialog(new TextPrompt($"{nameof(EnglishDetailsDialog)}.county"));
            AddDialog(new ChoicePrompt($"{nameof(EnglishDetailsDialog)}.subcounty"));
            AddDialog(new ChoicePrompt($"{nameof(EnglishDetailsDialog)}.ward"));
            // AddDialog(new TextPrompt($"{nameof(EnglishDetailsDialog)}.location"));
            // set the starting dialog 
            InitialDialogId = $"{nameof(EnglishDetailsDialog)}.mainFlow";
        }
        private async Task<DialogTurnResult> SelectedLanguageAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.name",
             new PromptOptions
            {
                Prompt = MessageFactory.Text("Which is preferred language ?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "English","Kiswahili" }),

            }, cancellationToken);
        }
        private async Task<DialogTurnResult> RegisterNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {   
             stepContext.Values["preferredLanguage"] = ((FoundChoice)stepContext.Result).Value;
            if((string) stepContext.Values["preferredLanguage"] == "English"){
             return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.name",
            new PromptOptions
            {
                Prompt = MessageFactory.Text("Welcome to our service, we would like to ask you" +
                " a few questions for the purpose of registration. What is your full name?:"),

            }, cancellationToken);
            }
            else{
                return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.name",
            new PromptOptions
            {
                Prompt = MessageFactory.Text("Karibu katika service yetu ,Jina lako ni ?:"),

            }, cancellationToken);
                
            }
        }


        private async Task<DialogTurnResult> CountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["name"] = (string)stepContext.Result;
            return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.county",
            new PromptOptions
            {
                Prompt = MessageFactory.Text("Which county do you live in?"),
                  }, cancellationToken);
        }
        private async Task<DialogTurnResult> SubCountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["county"] = (string)stepContext.Result;
            return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.subcounty",
            new PromptOptions
            {
                Prompt = MessageFactory.Text("Which sub-county are you located at?:"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "kiambu", "Eldoret", "Naivasha", "Nakuru", "MayCorn" }),

            }, cancellationToken);
        }


        private async Task<DialogTurnResult> WardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // value we get back from the choice prompt
            stepContext.Values["subcounty"] = ((FoundChoice)stepContext.Result).Value;
            return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.ward",

            new PromptOptions
            {
                Prompt = MessageFactory.Text("Which ward do you live? :"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "kiambu1", "Eldoret1", "Naivasha1", "Nakuru1", "MayCorn" }),

            }, cancellationToken);
        }

        //value we get back from the choice prompt
        private async Task<DialogTurnResult> ConfirmationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
            stepContext.Values["ward"] = ((FoundChoice)stepContext.Result).Value;

            var userDetails= await _botStateService.UserDetailsAccessor.GetAsync(stepContext.Context, () => new UserDetails(), cancellationToken);

                 userDetails.Language = (string) stepContext.Values["preferredLanguage"];
                 userDetails.FullName = (string) stepContext.Values["name"];
                 userDetails.County = (string) stepContext.Values["county"].ToString();
                 userDetails.SubCounty = (string) stepContext.Values["subcounty"];
                 
                 userDetails.Ward = (string)stepContext.Values["ward"];
            // Show the summary to the user 
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($" Congratulations {userDetails.FullName}! You are now" +
                $" registered to use our service.. Please choose (1. MAIN MENU) to continue using the service,"),cancellationToken);
          
            // save data in user state
            await _botStateService.UserDetailsAccessor.SetAsync(stepContext.Context, userDetails);
             // waterfall step always finishes eith the end of the waterfall so here is where it ends
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        // our validators...


            }
}