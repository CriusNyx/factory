# TODO

- [x] Support for negative products
- [x] Support for biproducts
- [x] Support for tally operator
- [x] Support for inline tally
- [x] Support for modifying recipes at call time
- [x] Support for variables
- [x] Support for anonymous recipes
- [ ] Support for units
- [x] Support for limit keyword
- [ ] Support for production lines using the -> operator???
  - Is this what I want? I don't think so? Consider doing a keyword based solution instead. Keywords are cool for DSL's.
- [n/a] Fixing the equality operator by making parser less greedy
- [n/a] Remove var keyword
  - Rejected. Using a keyword based paradigm for the language grammar where each statement begins with a keyword.
- [ ] Add a depth keyword
- [ ] Improve command line experience
- [x] Add more rich support for assign and invocation
- [ ] Add error handling to parser. It should fail if it does not consume all lexons.