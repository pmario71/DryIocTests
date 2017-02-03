# Design Goals

  1) Improve "Separation of Concern"

  1) Improve Functional Cohesion

  1) Testability

  1) Use DependencyInjection as a means to realize above points

## DependencyInjection

* we start with using MEF as our DI framework (yes, MEF is not officically DI)
  * multiple other alternatives available that are supperior with regards to:
    * perforance
    * usability
    * features 

* isolate DI framework from the rest of the code base so that it can be replaced with minor impact
  * do not have MEF-specific types injected
    (Attributes are OK because of their optional character)
  * avoid too much MEF-specific helper classes

* one single `CompositionRoot` for all Adapters/Services hosted in WorkflowServer
  (<http://blog.ploeh.dk/2011/07/28/CompositionRoot/>)

* Ambient Context for Cross-Cutting Concerns
  * identify types that qualify

* use Decorator Pattern for disposable types

* Lifecycle Scopes (per call, per session)

### DependencyInjection with Workflow

* only property injection might be possible

### Cohesion

Definition in Wikipedia: <https://en.wikipedia.org/wiki/Cohesion_(computer_science)>

In Workflow functionality is spread over a huge number of `Activities`. An `Activity` corresponds to a method on an interface or operation on a service.
However, with the difference that there is no way for a consumer to understand, which `Activities` belong semantically together.
The namespace could help here, but this is consistently enforced.

#### Functional Cohesion over Logical Cohesion

Activities are grouped along logical boundaries (accessing db, ...) rather than functionality.

There are a lot of similarly named `Activities` where a consumer has a hard time to findout what the `Activity` is doing under the hood and
 in which context (e.g. Torus, Cube or via) the `Activity` are intended to be used.

### Testability


### Normal vs. long running Operations

Timeouts on client side need to be "consistent" with service-side timeouts:

* clients should not timeout, because there is no background information we can present to the user
* if server timesout, we atleast send info which underlying operation did not complete

If there are operations that cannot complete with the typical 30s (WCF default) or are know to have a large variance for their response times, 
the interface to the operation needs to modelled differently:

```cs
Task<RequestHandle> rqh = client.BeginOpen();
client.SubscribeToChangeNotifications(rqh);

//or

//RequestHandle already allows to get change notifications / completion state
```

### Connection Reestablishment

* callback channels are also reestablished
* risk of missed callbacks! Resync necessary
  * more details required how to do Resync
  * Queries return lates information, while events a async and can lag behind