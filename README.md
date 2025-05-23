# Console trading program for Bybit

This repository contains a small console application written in C# using .NET 8.0. It demonstrates reading and aggregating trading data from Bybit.

Please note that this is a small vibe coding test using OpenAI's Codex.

## Features
- Reads trade data from compressed CSV files located in `ct/ct/bybit`.
- Provides an extensible `DataSource` class with an asynchronous message stream.
- Emits trade, candle, time and end-of-data messages.
- Aggregates trades into one-minute candles.
- Unit tests with xUnit verify chronological order and volume totals.
- GitHub Actions workflow builds the project and runs each test individually.

## Building and Running
1. Install .NET 8.0.
2. Build the solution:
   ```bash
   dotnet build ct/ct.sln
   ```
3. Run the console application:
   ```bash
   dotnet run --project ct/ct
   ```
   The program prints messages from the Bybit data file to the terminal.
4. Run all tests:
   ```bash
   dotnet test ct/ct.tests
   ```

## Project History
- **Initial commit** (May 22, 2025): basic boilerplate and message infrastructure.
- **Virtual message stream**: added an asynchronous method to `DataSource`.
- **Time and EndOfData messages**: introduced to signal gaps and completion.
- **Trade direction and chronological ordering**: improved message logic.
- **Candle structure**: added candles and candle messages.
- **Test files and unit tests**: included sample data and xUnit tests.
- **GitHub CI**: added workflow to run tests on every push.
- **Trade aggregation optimization**: grouped trades with identical timestamps.

The repository continues to evolve but currently serves as a minimal example of processing Bybit trade data in .NET.

## License
MIT
