When adding new public members or editing existing ones, please ensure that you update the XML documentation comments for those members.
This helps maintain code readability and provides useful information for other developers who may use or maintain the code in the future.

Additionally, if you are adding new features or making significant changes, consider updating any relevant unit tests to cover the new functionality and ensure that existing tests still pass.

All changes need to be unit testable. Favor composition over inheritance when designing new classes or features, and ensure that your code adheres to SOLID principles for maintainability and scalability.
When referencing classes like System.IO, System.Net.Http or similar, try to use a abstraction layer or interface to allow for easier testing and flexibility in the future.
This will help decouple your code from specific implementations and make it easier to mock dependencies in unit tests.

Give me a brief summary of the changes you are making, including the purpose and any relevant details, so that I can provide more specific guidance or suggestions if needed.

If you see any areas of the codebase that could benefit from refactoring or improvement, please feel free to suggest those changes as well.
This could include improving code readability, reducing complexity, or enhancing performance.
All suggestions should be accompanied by a clear rationale and, if possible, examples of how the changes would improve the codebase.
Suggestions need to be manually approved, so do not implement those by yourself without prior discussion and approval.

Finally, please make sure to run all existing tests and any new tests you create to verify that your changes do not introduce any regressions or issues in the codebase.