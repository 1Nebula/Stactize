
# Introduction

This repository contains example implementations of orchestrators that will consume messages from Stactize and send a response back.
At the moment, the only orchestrator example that has been created is the Durable Function Orchestrator example. 
  

# Getting Started

The Durable Function Orchestrator is designed around Azure Durable Functions, which are a special type of Azure Function that allow for a stateful workflow and support for longer lived functions. More information about Durable Functions can be found at https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp.

## 1. Prerequisites
In order to build and test the Durable Function Orchestrator Example, you must have the following software installed:

 1. NET Core 3.1 SDK
 2. Azure Functions Core Tools

Visual Studio Code, Visual Studio 2019 and Visual Studio 2022 support development for Durable Functions. You can see [Microsoft's Guide on creating a durable function](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-create-first-csharp?pivots=code-editor-vscode) for more information.

## 2. Understanding the Durable Function Orchestrator
There are three parts to the durable orchestrator: 
1. A service bus queue trigger 
2. An Orchestrator Run method
3. Activity Functions

When a message is placed on your ingress queue, the `TriggerOrchestrator` function will run. This function is responsible for starting the Durable Orchestrator. You can also use this function for data validation and early message rejection. 

> Note: If an exception is thrown while processing the message, it will be placed back on the queue for re-processing. This will happen ten times before dead-lettering. If you do not want this functionality, you should catch the exception and abandon the message.

The Durable Orchestrator is started by calling the `StartNewAsync` method on the `IDurableOrchestrationClient`. This will run the `RunOrchestrator` method.

In the example, the `RunOrchestrator` method determines which activity to call based on the `SubscriptionEvent` that is present on the incoming `OrchestrationActionModel`. This allows the Durable Orchestrator to call another function and keep its state by awaiting a `CallActivityAsync` call. This can easily be extended to accommodate more complicated scenarios by adding any required activity functions and awaiting them. Please see the tutorials for [Function Chaining](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-sequence?tabs=csharp), [Fan-out/Fan-in](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-cloud-backup?tabs=csharp), and [Human verification](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-phone-verification?tabs=csharp) for some examples of how to leverage durable functions.

Once the activity function has completed the orchestration action, the orchestrator needs to send a response back to Stactize. This response is expected to be of type `OrchestrationResultModel`. The returned result can either be a successful result (`OrchestrationState = Succeeded`) or an unsuccessful result (`OrchestrationState = Failed`). If any exception occurs in the orchestration or if there is a process that fails that will completely halt the orchestration, you should send a result back to Stactize to notify the user or an administrator (depending on the orchestration action). 
### Emails sent 
Stactize sends various emails after actions have been consumed and returned to the `Egress Queue`. Emails are sent to the following people according to whether the result is a success or not: 
| Action   | Success | Failure      | 
| --       | --      | --           |
| Create   | User    | User & Admin |
| Update   | User    | User & Admin |
| Suspend  | User    | Admin        |
| Reinstate| User    | Admin        |
| Delete   | User    | Admin        |

## 3. Adding your own orchestration code
In the `DurableFunctionOrchestrator` class, there are five activities that are triggered based on the `SubscriptionEvent` that is received by the durable orchestrator function. These activities are:

 1. `CreateInfrastructure`
 2. `DeleteInfrastructure`
 3. `Reinstate`
 4. `Suspend`
 5. `Update`

Add all the code required to execute the operation.
It is important to note that Stactize expects a response for all actions initiated - even if they fail. This is why each of these activity functions return a `OrchestrationResultModel`. If an exception is thrown inside the activity, it is recommended to catch the exception and return a default result model with the status set to failed (`OrchestrationState = Failed`).

### 3.1. Action Details
Each action type can come with more information that is specific to the action. This is stored as a dictionary of Key/Value strings in the `ActionDetails` property of the `OrchestrationActionModel`. 
#### 3.1.1 Create Action Details
```
"ActionDetails": 
{
    "AutoRenew" : "True", //If set to false, the Subscription will be cancelled at the end of the billing period
    "IsFreeTrial" : "False" //If set to true, the Subscription is in free trial and will not be charged for the first billing period
}
``` 

> Note: Action Details have currently not been configured for actions other than Create.

## 4. Testing your orchestrator
### 4.1 Local testing
For testing locally while developing, we recommend creating a test service bus with queues that match your ingress and egress queue names. You can then use a tool such as [Service Bus Explorer](https://github.com/paolosalvatori/ServiceBusExplorer) to manually add test messages to the queue.

To configure the Orchestrator to use these queues, update the `local.settings.json` file with the correct connection strings and queue names. 

Below is an example of a 'Create' event that should be sent to the `Ingress Queue` queue:

```
{
  "ActionDetails" : //See section 3.1. Action Details
  {
    "AutoRenew" : "True",
    "IsFreeTrial" : "False"
  },
  "ApplicationId": "b48463a8-1811-4c23-9f75-a6825c29f71c", //Id of Application in Stactize
  "OperationId": "a1a644f5-efdf-43f3-b98e-01c5b55a6a98", //Id of the Operation
  "TenantId": "bd622fe3-ea28-42e9-ad12-56b5be8536c1", //Id of the Tenant requesting the action
  "SubscriptionId": "7d8e2ac6-80ce-4568-8063-a862081701e1", //Id of the Subscription requesting the action
  "PlanId": "planId", //Id of the plan chosen by the subscriber
  "Event": "Create",
  "UserFields": [
    {
      "Name": "FirstName",
      "Value": "Robert"
    },
    {
      "Name": "LastName",
      "Value": "Tables"
    },
    {
      "Name": "Email",
      "Value": "signup@email.com"
    },
    {
      "Name": "PreferredContactEmail",
      "Value": ""
    },
  ]
}
```

### 4.2 End-to-end Testing
Once local testing is complete, you can test the entire integration between the Azure Marketplace, Stactize and your orchestrator. To do this, ensure the following prerequisites have been completed:

#### Prerequisites
1. A SaaS offer setup in the Microsoft Partner Center
2. Your SaaS application set up in Stactize
3. Technical configuration setup correctly in the Microsoft Partner Center
4. The SaaS offer at least in Preview
5. Your Orchestrator Ingress and Egress queues setup correctly based on the Stactize Orchestrator Configuration for your application
 
> Note:  We recommend creating a zero-cost private plan that is only visible to members of your company for testing purposes.

You can then configure your Orchestrator to use the ingress and egress queues specified by the `Orchestrator Configuration` found in Stactize. If your Orchestrator is deployed to Azure, ensure the *ingressServiceBusConnectionString*, *ingressQueueName*, *egressServiceBusConnectionString* and *egressQueueName* [application settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings?tabs=portal#settings) have been setup in the Portal.

With the prerequisites completed, you can then search for your application in the Marketplace and purchase it. Stactize should send a "Create" message to your application's `Ingress Queue`. If you purchase a "Preview" offer, the message will be sent to the "Preview" queue, otherwise it will be sent to the "Production" queue. The subscription can then be updated and cancelled to test the other events. If you subscribe to a non-zero cost plan, you will not be charged for the subscription if you cancel it before 24 hours for a monthly billed plan or 14 days for a yearly billed plan.

## 5. Support
Please don't hesitate to send an email to support@1Nebula.com for more information or to answer any questions.