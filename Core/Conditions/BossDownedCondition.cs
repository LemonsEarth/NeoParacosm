using Terraria.GameContent.ItemDropRules;

namespace NeoParacosm.Core.Conditions
{
    public class KingSlimeDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedSlimeKing;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class EyeOfCthulhuDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedBoss1;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class QueenBeeDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedQueenBee;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class SkeletronDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedBoss3;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class DeerclopsDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedDeerclops;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class DestroyerDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedMechBoss1;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class TwinsDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedMechBoss2;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class PrimeDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedMechBoss3;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class QueenSlimeDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedQueenSlime;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class PlanteraDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedPlantBoss;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }

    public class GolemDowned : IItemDropRuleCondition, IProvideItemConditionDescription
    {
        public bool CanDrop(DropAttemptInfo info) => NPC.downedGolemBoss;
        public bool CanShowItemDropInUI() => true;
        public string GetConditionDescription() => null;
    }
}
