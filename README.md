
# Introduction

This repository contains example implementations of orchestrators that will consume messages from Stactize and send a response back.
At the moment, the only orchestrator example that has been created is the Durable Function Orchestrator example. 
  

# Getting Started

The Durable Function Orchestrator is designed around Azure Durable Functions, which are a special type of Azure Function that allow for a stateful workflow and support for longer lived functions. More information about Durable Functions can be found at https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp.

## 1. Prerequisites
In order to build and test the Durable Function Orchestrator Example, you must have the following software installed:

 1. NET Core 3.1 SDK
 2. Azure Functions Core Tools

Both Visual Studio Code and Visual Studio 2019 support development for Durable Functions. You can see [Microsoft's Guide on creating a durable function](https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-create-first-csharp?pivots=code-editor-vscode) for more information.

If you wish to test orchestration for a preview Azure Marketplace offering, you will also require:
1. A SaaS offering setup and in Preview
2. Your SaaS application configured in Stactize
3. The Service Bus connection string for your ingress queue
4. The Service Bus connection string for your egress queue

You will be sent the two connection strings after the application has been configured in Stactize.

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

## 4. Testing your orchestrator
Your orchestrator can be tested in multiple ways. For testing locally while developing, we recommend creating a test service bus and queues and manually adding messages to it. Below is an example of a 'Create' event that should be sent to the `orchestrator-myawesomeapp` queue:

> {
    "OperationId": "65a76f35-a492-46de-acdd-ed87bd37b8ac",
    "TenantId": "b98027f5-3949-4905-b908-9f4f38192d9c",
    "SubscriptionId": "e6005cdd-24fa-4c9a-88e7-1c3a32e0e20e",
    "ApplicationId": "myawesomeapp",
    "Event": "Create",
    "UserFields": [
        {
            "name": "FirstName",
            "value": "Robert"
        },
        {
            "name": "LastName",
            "value": "Tables"
        },
        {
            "name": "Email",
            "value": "bobby@tables.co.za"
        }
    ]
}

You can also test your integration by having an application setup with Stactize already and in preview in the Azure Marketplace. You can then sign up, update and cancel a subscription. You will not need to pay for the subscription if you cancel it before 24 hours for a monthly billed plan or 14 days for a yearly billed plan.
In the future, you will be able to test your integration by initiating actions from the Stactize API. This will generate mock events for your application. 

## 5. Support
Please don't hesitate to send an email to support@48software.com for more information or to answer any questions.