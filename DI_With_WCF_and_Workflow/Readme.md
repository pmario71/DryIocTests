﻿# Readme

## Design Goals

  1) Improve "Separation of Concern"

  1) Improve Cohesion

  1) Testability

## Use DependencyInjection



### Cohesion

Definition in Wikipedia: <https://en.wikipedia.org/wiki/Cohesion_(computer_science)>

In Workflow functionality is spread over a huge number of `Activities`. An `Activity` corresponds to a method on an interface or operation on a service.
However, with the difference that there is no way for a consumer to understand, which `Activities` belong semantically together.
The namespace could help here, but this is consistently enforced.

#### Functional Cohesion over Logical Cohesion

Activities are grouped along logical boundaries (accessing db, ...) rather than functionality.

There are a lot of similarly named `Activities` where a consumer has a hard time to findout what the `Activity` is doing under the hood and
 in which context (e.g. Torus, Cube or via) the `Activity` are intended to be used.
