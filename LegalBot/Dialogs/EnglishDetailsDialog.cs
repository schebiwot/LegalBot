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
 
    // It inherits from parent class 
  
    
    public class EnglishDetailsDialog :ComponentDialog
    {
        private readonly BotStateService _botStateService;
        private string jsonFile = @"Json/county.json";
        public  string county ="";
        public string sub_county = "";
        public bool  registered = false;
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
            {   
                // BeginRegistrationAsync,
                SelectedLanguageAsync,
                RegisterNameAsync,
                CountyStepAsync,
                SubCountyStepAsync,
                WardStepAsync,
                ConfirmationStepAsync,
                FinalMenuAsync,
               
            };

            AddDialog(new WaterfallDialog($"{nameof(EnglishDetailsDialog)}.mainFlow", waterfallSteps));
            AddDialog(new ChoicePrompt($"{nameof(EnglishDetailsDialog)}.preferredLanguage"));
            AddDialog(new TextPrompt($"{nameof(EnglishDetailsDialog)}.name"));
            AddDialog(new NumberPrompt<int>($"{nameof(EnglishDetailsDialog)}.county"));
            AddDialog(new NumberPrompt<int>($"{nameof(EnglishDetailsDialog)}.subcounty"));
            AddDialog(new NumberPrompt<int>($"{nameof(EnglishDetailsDialog)}.ward"));
            AddDialog(new ChoicePrompt($"{nameof(EnglishDetailsDialog)}.mainMenu"));
            AddDialog(new MainMenuDialog($"{nameof(EnglishDetailsDialog)}.mainMenuDialog", _botStateService));
            
            


            // set the starting dialog 
            InitialDialogId = $"{nameof(EnglishDetailsDialog)}.mainFlow";
        }
        // private async Task<DialogTurnResult> BeginRegistrationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        // {
            
              
        //         string bye = "Hello my name is Kuza,I give legal information";
        //         await stepContext.Context.SendActivityAsync(bye);
        //         return await stepContext.BeginDialogAsync($"{nameof(EnglishDetailsDialog)}.mainFlow", null, cancellationToken);
        
        // }

        private async Task<DialogTurnResult> SelectedLanguageAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {  
            // Ask the user to enter there preferred language
            var promptOptions = new PromptOptions {
                Prompt = MessageFactory.Text("Hujambo, karibu katika huduma yetu. Tafadhali chagua lugha inayokufaa (1. KISWAHILI),  (2. KINGEREZA)" +
                "Hello, welcome to our service. Please choose your preferred language (1. KISWAHILI),  (2. ENGLISH)"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "English","Kiswahili" }),

            };
            //  Asks the user to enter there name 
            return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.preferredLanguage",promptOptions,cancellationToken);
        }
        private async Task<DialogTurnResult> RegisterNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {   
            // Set's the users preferred language to what they entered 
            stepContext.Values["preferredLanguage"] = ((FoundChoice)stepContext.Result).Value;

            if((string) stepContext.Values["preferredLanguage"] == "English"){

                var promptOptions = new PromptOptions {
                    Prompt = MessageFactory.Text("Welcome to our service, we would like to ask you" +
                    "a few questions for the purpose of registration. What is your full name?:"),

                };

                return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.name",promptOptions, cancellationToken);

            }

            else {
                var promptOptions = new PromptOptions {
                Prompt = MessageFactory.Text("Karibu katika service yetu ,Tungependa kukuliza maswali ndio tuweze" +
                "kukusajili ,Jina lako ni ?")};
                
                return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.name",promptOptions, cancellationToken);
                
            }
        }


        private async Task<DialogTurnResult> CountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {   
            // setting the users name 
            stepContext.Values["name"] = (string)stepContext.Result;
            
          
            //  reading data from the json file
            var json = File.ReadAllText(jsonFile);
            
;

            try{
                

              var counties = JsonConvert.DeserializeObject<List<County>>(json);

                string options ="";
                

                for(int i =0; i<counties.Count; i++){

                            options += $"\n{i+1}.{counties[i].CountyName}\n";
                } 

                if((string) stepContext.Values["preferredLanguage"] == "English"){
                        
                        
                        
                        var promptOptions =  new PromptOptions{
                                Prompt = MessageFactory.Text("Which county do you live in(select a county number) ?\n" + options),
                                RetryPrompt = MessageFactory.Text("Please choose an option from the list."),

                        };
                        return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.county", promptOptions, cancellationToken);
                }
            
                else 
                {
                    var promptOptions = new PromptOptions {
                    Prompt = MessageFactory.Text("Unaishi katika Kaunti gani ?\n" + options),
                    RetryPrompt = MessageFactory.Text("Tafadhali chagua kutoka kwenye list"),

                    };
                    return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.county",promptOptions, cancellationToken);
                }
            }

            catch (Exception)  
            {  
  
                 throw;  
            }
            
        }
        private async Task<DialogTurnResult> SubCountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["county"] = (int)stepContext.Result;
            var json = File.ReadAllText(jsonFile);
            var counties = JsonConvert.DeserializeObject<List<County>>(json);

               
                int id = (int)stepContext.Values["county"];
              
                string sub_counties = "";
               
                
                for(int i =0; i<counties.Count; i++){ 
                    

                            if(counties[i].CountyId == id) 
                            {
                                county += counties[i].CountyName;
                                
                                 for(int j = 0; j< counties[i].SubCounties.Length;j++){
                                   sub_counties += $"\n{j+1}.{counties[i].SubCounties[j].Name}\n";
                                  
                               }
                            }
                            
                }
             
                
                
            if((string) stepContext.Values["preferredLanguage"] == "English"){
                
                var promptOptions = new PromptOptions{

                    Prompt = MessageFactory.Text("Which sub-county are you located at?\n" + sub_counties),
                    RetryPrompt = MessageFactory.Text("Please choose an option from the list."),
                   

                };
                return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.subcounty",promptOptions, cancellationToken);
            }
            else {
               
                var promptOptions = new PromptOptions {
                    Prompt = MessageFactory.Text("Unaishi Katika sub-kaunti gani ?:\n"+ sub_counties),
                   RetryPrompt = MessageFactory.Text("Tafadhali chagua kutoka kwenye list"),

                };
                
                 return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.subcounty",promptOptions, cancellationToken);

            }
        }


        private async Task<DialogTurnResult> WardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // value we get back from the choice prompt
            stepContext.Values["subcounty"] = (int)stepContext.Result;
            
              int sub_id = (int)stepContext.Values["subcounty"];
              var json = File.ReadAllText(jsonFile);
              var counties = JsonConvert.DeserializeObject<List<County>>(json);
              string [] ward_arr = new string[] {};
              string ward_str = "";
               
                int county_id = (int)stepContext.Values["county"];
              
                for(int i =0; i<counties.Count; i++){ 
                    

                            if(counties[i].CountyId == county_id) 
                            { 
                               
                              for(int j = 0; j< counties[i].SubCounties.Length;j++){
                                  sub_county = counties[i].SubCounties[j].Name;
                                  
                                   if(counties[i].SubCounties[j].Id == sub_id){
                                       ward_arr = counties[i].SubCounties[j].Wards;
                                }
                                        
                                   }
                            }
                } 
                
                for(int k = 0; k < ward_arr.Length; k++){
                   ward_str +=$"\n {k+1}. {ward_arr[k]}\n";
                }

                            
                


            if((string) stepContext.Values["preferredLanguage"] == "English"){

              var promptOptions =  new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which ward do you live? :\n"+ ward_str),
                    RetryPrompt = MessageFactory.Text("Please choose an option from the list."),

                };
                return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.ward",promptOptions,cancellationToken);
            }
            else {
                

               var promptOptions= new PromptOptions
                {
                    Prompt = MessageFactory.Text("Unaishi Katika Ward gani ?\n"+ ward_str),
                    RetryPrompt = MessageFactory.Text(" Tafadhali chagua option kutoka list."),

                };
                
                return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.ward",promptOptions
                ,cancellationToken);
            }
        
        }
        private async Task<DialogTurnResult> ConfirmationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken) {
           stepContext.Values["ward"] = (int)stepContext.Result;
            
           var userDetails= await _botStateService.UserDetailsAccessor.GetAsync(stepContext.Context, () => new UserDetails(), cancellationToken);

                userDetails.Language = (string) stepContext.Values["preferredLanguage"];
                userDetails.FullName = (string) stepContext.Values["name"];
                userDetails.County = county;
                userDetails.SubCounty =sub_county;

            await _botStateService.UserDetailsAccessor.SetAsync(stepContext.Context, userDetails);
          

           // Show the summary to the user 
           if ((string) stepContext.Values["preferredLanguage"] == "English"){

               var promptOptions = new PromptOptions {
                   Prompt = MessageFactory.Text($" Congratulations {userDetails.FullName}! You are now" +
                   $"registered to use our service.. Please choose (1. MAIN MENU) to continue using the service"),

                  Choices = ChoiceFactory.ToChoices(new List<string> { "1.MENU", "2.EXIT" }),

               };

             
               return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.mainMenu", promptOptions, cancellationToken);

           }
           else{
               var promptOptions = new PromptOptions {
                Prompt = MessageFactory.Text($"Karibu {userDetails.FullName}! tumekusajili" +
                $" Katika service yetu.. Tafadhali chagua (1. MAIN MENU) ndio uweze kuendelea kutumia service zetu...,"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "1.MENU", "2.EXIT" }),
               };

               return await stepContext.PromptAsync($"{nameof(EnglishDetailsDialog)}.mainMenu", promptOptions, cancellationToken);
                  
               };
               
           
        }

        private async Task<DialogTurnResult> FinalMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["mainMenu"] = ((FoundChoice)stepContext.Result).Value;

            if((string) stepContext.Values["mainMenu"] == "1.MENU")
            {

                return await stepContext.BeginDialogAsync($"{nameof(EnglishDetailsDialog)}.mainMenuDialog", null, cancellationToken);
            }
            else
            {
              
                string bye = "GoodBye";
                await stepContext.Context.SendActivityAsync(bye);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }
        






    }
}
