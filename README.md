# CLI Blackjack Duel

CLI Blackjack Duel is a command-line card battle game implemented in F# using .NET 10.

The game is based on simplified Blackjack rules. Unlike ordinary Blackjack, this game removes all betting-related rules and replaces betting with an HP-based battle system. The player and the computer each start with 100 HP, and the game continues until one side reaches 0 HP.

## Project Overview

In each round, the player and the computer receive cards according to Blackjack rules. The player can choose whether to draw more cards or stop. The computer follows a fixed dealer-like rule and draws cards until its total is at least 17.

After both sides finish their turns, the round winner is decided. Instead of winning or losing money, the winner deals damage to the loser. This makes the game a turn-based Blackjack battle.

## Features

- Command-line interface
- Simplified Blackjack rule system
- HP-based battle system
- Player action choices: Hit or Stand
- Computer-controlled opponent
- Blackjack damage rule
- Bust handling
- Draw condition for simultaneous Blackjack damage
- Repeated rounds until the game ends

## Requirements

- F#
- .NET 10

## How to Run

Clone this repository and run the project with:

bash dotnet run 

## How to Play

At the start of the game, both the player and the computer have 100 HP.

During the player's turn, enter one of the following options:

text 1. Hit 2. Stand 

- Hit draws one additional card.
- Stand ends the player's turn.

After the player's turn, the computer takes its turn automatically.

## Card Rules

The game uses standard simplified Blackjack card values:

- Cards from 2 to 10 have their printed values.
- J, Q, and K count as 10.
- A counts as 11 unless that would make the total exceed 21. Otherwise, A counts as 1.
- A Blackjack is a two-card hand with a total value of 21.

## Computer Rule

The computer follows a fixed dealer-like rule:

- If the computer's total is less than 17, it draws another card.
- If the computer's total is 17 or higher, it stands.

## Damage Rules

The game does not use betting, chips, or money.

Instead, damage is calculated as follows:

- If exactly one side has Blackjack, that side deals 21 damage to the opponent.
- If both sides have Blackjack, both sides take 21 damage.
- In all non-Blackjack cases, the winner deals damage equal to the absolute difference between the player's final total and the computer's final total.
- If neither side has Blackjack and both final totals are equal, no damage is dealt.

Busts are also handled using the same non-Blackjack damage rule. For example, if the player has 24 and the computer has 18, the computer wins and deals |24 - 18| = 6 damage.

## Win, Lose, and Draw Conditions

- The player wins if the computer reaches 0 HP first.
- The player loses if the player reaches 0 HP first.
- If both sides reach 0 HP at the same time because both sides had Blackjack, the game ends in a draw.
- If both sides have Blackjack but both still have HP remaining, the game continues to the next round.

## Implementation Notes

The implementation is written in a single F# source file, Program.fs.

The program includes:

- algebraic data types for suits, ranks, cards, and round results,
- deck creation and shuffling,
- Blackjack hand value calculation with Ace adjustment,
- player input handling,
- computer turn logic,
- round result calculation,
- HP damage application,
- recursive game loop for repeated rounds.

The implementation intentionally avoids complex external dependencies so that the game can be run directly with dotnet run.

## LLM Usage

I used a large language model to help organize the requirements, write the initial F# implementation, and prepare the README.

I manually reviewed and adjusted the game rules so that the implementation matches the submitted requirements. In particular, I checked the HP damage system, bust handling, Blackjack damage, and the draw condition for simultaneous Blackjack damage.

The main difficulty was ensuring that the generated code followed the exact project requirements for edge cases such as busts, Blackjack, and simultaneous damage.