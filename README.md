# Timer

    Timer implementation that doesn't use the built in .Net Timer classes.

## Table of Contents

* [About the Project](#about-the-project)
  * [Built With](#built-with)
* [Getting Started](#getting-started)
  * [Installation](#installation)
* [Usage](#usage)
  * [Example](#example)
* [License](#license)
* [Contact](#contact)

## About The Project

The solution contains the timer library, a project for some tests, and a project for a console application that uses the library.

### Built With

* C# in Visual Studio 2019

## Getting Started

### Installation

1. Clone the repo
```sh
git clone https://github.com/heathi/Timer.git
```
2. Open in Visual Studio and build solution.

## Usage

1. Initilize the Timer

```csharp
var timer = new AlternateTimer.Timer();
```
2. Register an event 

```csharp
timer.Elapsed += DoWork;
```

3. Start The Timer 

```csharp
timer.Start(seconds);
```

4. Stop the Timer

```csharp
timer.Stop();
```

### Example
```csharp

    class Program
    {
        static void Main(string[] args)
        {
            var seconds = 2;
            var iterations = 5;

            var timer = new AlternateTimer.Timer();
            timer.Elapsed += WriteToConsole;
            timer.Start(seconds);

            // Do Some Work
            Task.Delay(seconds * iterations * 1000).Wait();

            timer.Stop();

            Console.ReadLine();
        }

        private static void WriteToConsole(object sender, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            Console.WriteLine($"Current time: {DateTime.UtcNow}");
        }
    }
```

## License

Distributed under the MIT License. See `LICENSE` for more information.


## Contact

Project Link: [https://github.com/heathi/Timer](https://github.com/heathi/Timer)
