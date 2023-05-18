
namespace DurableFunctionOrchestratorExample
{
    public static class Constants
    {
        public const string DurableOrchestrator = "DurableOrchestratorFunction";
        public const string OrchestrationTrigger = "OrchestrationTriggerFunction";

        public static class DurableActivity
        {
            public const string Create = "DurableActivity_Create";
            public const string Update = "DurableActivity_Update";
            public const string Delete = "DurableActivity_Delete";
            public const string Suspend = "DurableActivity_Suspend";
            public const string Reinstate = "DurableActivity_Reinstate";
            public const string CompleteAction = "DurableActivity_CompleteAction";
        }

    }
}
