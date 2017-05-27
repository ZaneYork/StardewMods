using System.Collections.Generic;
using StardewValley;

namespace Entoarox.AdvancedLocationLoader
{
    internal class ConditionalResolver
    {
        private static Dictionary<string, ConditionalResolver> Cache = new Dictionary<string, ConditionalResolver>();
        internal static ConditionalResolver Request(string list)
        {
            if (!ConditionalResolver.Cache.ContainsKey(list))
                ConditionalResolver.Cache.Add(list, new ConditionalResolver(list));
            return ConditionalResolver.Cache[list];
        }
        private Configs.Conditional Conditional;
        private ConditionalResolver(string name)
        {
            this.Conditional = Configs.Compound.Conditionals.Find(e => e.Name == name);
        }
        private string GetItemName()
        {
            if (this.Conditional.Item == -1)
                return ModEntry.Strings["gold"];
            else
                return new StardewValley.Object(this.Conditional.Item, 1).Name;
        }
        internal void Init()
        {
            Response[] answers = new Response[2];
            answers[0] = new Response("y", ModEntry.Strings["yesCost"].Replace("{0}", Conditional.Amount.ToString()).Replace("{1}", GetItemName()));
            answers[1] = new Response("n", ModEntry.Strings["no"]);
            Game1.currentLocation.lastQuestionKey = "CompleteConditionalQuestion";
            Game1.currentLocation.createQuestionDialogue(this.Conditional.Question, answers, this.Resolver, null);
        }
        internal void Resolver(StardewValley.Farmer who, string answer)
        {
            if (answer == "n")
                return;
            if ((this.Conditional.Item == -1 && who.money >= this.Conditional.Amount) || who.hasItemInInventory(this.Conditional.Item, this.Conditional.Amount))
            {
                if (this.Conditional.Item == -1)
                    who.money -= this.Conditional.Amount;
                else
                    who.removeItemsFromInventory(this.Conditional.Item, this.Conditional.Amount);
                who.mailReceived.Add("ALLCondition_" + this.Conditional.Name);
                ModEntry.Logger.Log("Conditional completed: " + this.Conditional.Name, StardewModdingAPI.LogLevel.Trace);
                ModEntry.UpdateConditionalEdits();
                if (this.Conditional.Success != null)
                    Game1.drawDialogueBox(this.Conditional.Success);
            }
            else
                Game1.drawObjectDialogue(ModEntry.Strings["notEnough"].Replace("{0}",GetItemName()));

        }
    }
}
