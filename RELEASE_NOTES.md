### 4.0
* Added Trace and Notice levels
* Move to netstandard 2.1
* Fix breaking changes in 3.1

### 3.1
* Add fast-path optimization to loggers without a consumers.
* Add API to remove consumers from the logger.

### 3.0
* Use format strings for `appendPath` and `withPath`
* Logger module fixes

### 2.5
* Target net472

### 2.4
* Added Xplat way to get a long time string

### 2.3
* Added StructedFormatDisplay attribute
* Adjusted default string representation
* Improved appendPath function

### 2.2
* Remove extra newline on end of ShortString

### 2.1
* Fix broken ShortString
* Add unit test for ShortString

### 2.0
* Fixed path display to always use '/' separators
* Added short forms
* Renamed Printfn logger to Console logger
* Added colour logger

### 1.3
* Added shorthand calls for log levels.
* Adjusted default PrintFn logger to print to std::err for warn, error and fatal levels.

### 1.2
* Moved to .Net 4.5.2
* Upgraded to Fsharp 4.1

### 1.1
* Enhanced documentation
* Added appendPath function to allow heirarchical pathing.

### 1.0
* Initial release of FSLogger.
