# How to do TDD

## Not confusing design and architecture

The following text discusses TDD as a mean to drive the design, not to settle the architecture. Architecture encompasses principle decisions made upfront; architecture should not change, changes should be confined to the design and to the implementation. Architecture covers the whole solution, design is per part. Design may be explorative, and TDD is a mean to explore possible solutions. Architecture affects desing, not vice versa. Architecture ensures the overal fit and quality, design covers the appropriate and necessary details.

## Driving the interface vs driving the implementation

Some like Uncle Bob demonstrate TDD to develop implementations. Others like David Farley demonstrate TDD to develop a clear and testable API.

Clearly the goal of the latter is simpler, and hence it should be easier to practice and quicker to give expected and useful results. Also, the former imposes the danger of writing tests that are coupled to the implementation.

## Driving the interface vs driving the syntax

When the API development is driven by tests, the tests will initially lack any concern for functionality and will instead focus on syntactic and structural decisions. Such tests do not have much value; the syntax will always remain arbitrary to some degree -- order and naming can easily change --, and structure should always be rather rigid -- changing the data model usually affects and maybe overturns everything else.

Syntax-focused tests could exist transiently, but should not become part of the essential suite of unit tests.

## Separate models

When developing a particular problem solution, keep it abstract from the domain. E.g. when developing a solution for some logistic problem for a railway application, where the domain contains train stations and train connections, don't let stations and connections appear in the logistic solution.

An often helpful indicator is: when solving a mathematical problem, don't waste time with names.

One possibility is to use generic programming, where the solution operates on interfaces or generics, and the objects of the application domain then implement the interfaces. Another possibility is to use specific solution models and to translate between application domain and the problem-specific domain. Either choice has its consequences on the final depencency relations.

## Refactor out

If the language and IDE support it well, do all manual changes in the test project, and then use automated refactoring to extract production code from the test code. This also encourages frequent cleanup-by-refactoring.

There is no strict rule how quickly or how often production code should be extracted from the test code.

## Integration vs Mocking

The term "mocking" is used ambiguously, either refering to the substitution of external dependencies -- external systems, plus all the things that Uncle Bob calls "IO devices" --, or refering to the substitution of internals -- this was originally called "endo-testing".

The latter invariably coincides with white-box testing and implies coupling tests to the implementation.

Mocking of external systems and so-called "IO devices" is in general good practice.

Mocking of internal dependencies is usually done to isolate one class or even one method of a class, and makes sense when testing the logic of that class or method. However, a class or method that can be sensibly tested in isolation usually tends to be large and complex, and is usually hard to refactor. It may be that avoiding white-box testing -- and thereby endo-testing with its mocking of internal dependencies -- will lead to a break-up of that class or method into smaller parts, that are more amenable to testing as well as refactoring.

For large classes or methods that are not expected to change -- e.g. when implementing a specific, well-established algorithm, or when aiming for hardcoded optimizations -- mocking of internals may be ok.

## Comment on Red-Green-Refactor Cycle

Uncle Bob often stated that Red-Green-Refactor leads to a short production cycle in the order of a minute or some.

That sounds to me as if it was only achievable if the solution is going to be highly linear/incremental/orthogonal, when every new test touches a completly new aspect without interfering with any cross-concern aspects. Ideally think of a soap bubble: all the time all sides are closely in ballance; forming a spike is not doable. On the other hand, when going by tests, it is easy to drill down into one problem aspect, to go for a spike.

To form a round bubble, knowing what-do-add-when (which aspect to increment next) must be known before. I think TDD is of no help as soon as cross-concerns come into play.
