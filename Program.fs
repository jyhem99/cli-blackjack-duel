open System

type Suit =
    | Hearts
    | Diamonds
    | Clubs
    | Spades

type Rank =
    | Two | Three | Four | Five | Six | Seven | Eight | Nine | Ten
    | Jack | Queen | King | Ace

type Card = {
    Rank: Rank
    Suit: Suit
}

type RoundResult =
    | PlayerWins of int
    | ComputerWins of int
    | BothBlackjack of int
    | NoDamage

let rng = Random()

let allSuits = [ Hearts; Diamonds; Clubs; Spades ]

let allRanks =
    [ Two; Three; Four; Five; Six; Seven; Eight; Nine; Ten
      Jack; Queen; King; Ace ]

let createDeck () =
    [ for suit in allSuits do
        for rank in allRanks do
            yield { Rank = rank; Suit = suit } ]

let shuffleDeck (deck: Card list) =
    deck
    |> List.sortBy (fun _ -> rng.Next())

let drawCard (deck: Card list) =
    match deck with
    | [] -> failwith "The deck is empty."
    | card :: remainingDeck -> card, remainingDeck

let rankToString rank =
    match rank with
    | Two -> "2"
    | Three -> "3"
    | Four -> "4"
    | Five -> "5"
    | Six -> "6"
    | Seven -> "7"
    | Eight -> "8"
    | Nine -> "9"
    | Ten -> "10"
    | Jack -> "J"
    | Queen -> "Q"
    | King -> "K"
    | Ace -> "A"

let suitToString suit =
    match suit with
    | Hearts -> "Hearts"
    | Diamonds -> "Diamonds"
    | Clubs -> "Clubs"
    | Spades -> "Spades"

let cardToString card =
    sprintf "%s of %s" (rankToString card.Rank) (suitToString card.Suit)

let cardBaseValue card =
    match card.Rank with
    | Two -> 2
    | Three -> 3
    | Four -> 4
    | Five -> 5
    | Six -> 6
    | Seven -> 7
    | Eight -> 8
    | Nine -> 9
    | Ten | Jack | Queen | King -> 10
    | Ace -> 11

let handValue hand =
    let total = hand |> List.sumBy cardBaseValue
    let aceCount =
        hand
        |> List.filter (fun card -> card.Rank = Ace)
        |> List.length

    let rec adjust value acesLeft =
        if value > 21 && acesLeft > 0 then
            adjust (value - 10) (acesLeft - 1)
        else
            value

    adjust total aceCount

let isBlackjack hand =
    List.length hand = 2 && handValue hand = 21

let printHand owner hand =
    let cards =
        hand
        |> List.map cardToString
        |> String.concat ", "

    printfn "%s cards: %s" owner cards
    printfn "%s total: %d" owner (handValue hand)

let dealInitialHands deck =
    let p1, deck = drawCard deck
    let c1, deck = drawCard deck
    let p2, deck = drawCard deck
    let c2, deck = drawCard deck
    [ p1; p2 ], [ c1; c2 ], deck

let rec playerTurn hand deck =
    printfn ""
    printHand "Your" hand

    if handValue hand > 21 then
        printfn "You busted."
        hand, deck
    else
        printfn ""
        printfn "Choose action:"
        printfn "1. Hit"
        printfn "2. Stand"
        printf "> "

        let input = Console.ReadLine()

        match input with
        | "1" ->
            let card, newDeck = drawCard deck
            printfn "You drew: %s" (cardToString card)
            playerTurn (hand @ [ card ]) newDeck
        | "2" ->
            printfn "You stand."
            hand, deck
        | _ ->
            printfn "Invalid input. Please enter 1 or 2."
            playerTurn hand deck

let rec computerTurn hand deck =
    let total = handValue hand

    if total < 17 then
        let card, newDeck = drawCard deck
        printfn "Computer draws a card."
        computerTurn (hand @ [ card ]) newDeck
    else
        printfn "Computer stands."
        hand, deck

let decideRound playerHand computerHand =
    let playerTotal = handValue playerHand
    let computerTotal = handValue computerHand

    let playerBlackjack = isBlackjack playerHand
    let computerBlackjack = isBlackjack computerHand

    if playerBlackjack && computerBlackjack then
        BothBlackjack 21
    elif playerBlackjack then
        PlayerWins 21
    elif computerBlackjack then
        ComputerWins 21
    elif playerTotal > 21 && computerTotal > 21 then
        let damage = abs (playerTotal - computerTotal)

        if playerTotal > computerTotal then
            ComputerWins damage
        elif computerTotal > playerTotal then
            PlayerWins damage
        else
            NoDamage
    elif playerTotal > 21 then
        let damage = abs (playerTotal - computerTotal)
        ComputerWins damage
    elif computerTotal > 21 then
        let damage = abs (playerTotal - computerTotal)
        PlayerWins damage
    elif playerTotal > computerTotal then
        let damage = abs (playerTotal - computerTotal)
        PlayerWins damage
    elif computerTotal > playerTotal then
        let damage = abs (playerTotal - computerTotal)
        ComputerWins damage
    else
        NoDamage

let applyDamage playerHp computerHp result =
    match result with
    | PlayerWins damage ->
        printfn "You win this round and deal %d damage." damage
        playerHp, computerHp - damage
    | ComputerWins damage ->
        printfn "Computer wins this round and deals %d damage." damage
        playerHp - damage, computerHp
    | BothBlackjack damage ->
        printfn "Both sides have Blackjack. Both take %d damage." damage
        playerHp - damage, computerHp - damage
    | NoDamage ->
        printfn "This round is a tie. No damage is dealt."
        playerHp, computerHp

let printRoundSummary playerHand computerHand =
    printfn ""
    printfn "Final hands:"
    printHand "Your" playerHand
    printfn ""
    printHand "Computer" computerHand
    printfn ""

let rec gameLoop playerHp computerHp roundNumber =
    printfn ""
    printfn "========================================"
    printfn "Round %d" roundNumber
    printfn "Player HP: %d" playerHp
    printfn "Computer HP: %d" computerHp
    printfn "========================================"

    let deck = createDeck () |> shuffleDeck
    let playerHand, computerHand, deck = dealInitialHands deck

    printfn ""
    printfn "Initial hands:"
    printHand "Your" playerHand
    printfn "Computer shows one card: %s" (cardToString computerHand.Head)

    let playerFinalHand, deckAfterPlayer =
        if isBlackjack playerHand then
            printfn ""
            printfn "You have Blackjack."
            playerHand, deck
        else
            playerTurn playerHand deck

    let computerFinalHand, _ =
        if isBlackjack computerHand then
            printfn ""
            printfn "Computer has Blackjack."
            computerHand, deckAfterPlayer
        else
            printfn ""
            printfn "Computer turn starts."
            computerTurn computerHand deckAfterPlayer

    printRoundSummary playerFinalHand computerFinalHand

    let result = decideRound playerFinalHand computerFinalHand
    let newPlayerHp, newComputerHp = applyDamage playerHp computerHp result

    printfn ""
    printfn "Updated HP:"
    printfn "Player HP: %d" newPlayerHp
    printfn "Computer HP: %d" newComputerHp

    match result with
    | BothBlackjack _ when newPlayerHp <= 0 && newComputerHp <= 0 ->
        printfn ""
        printfn "Both players reached 0 HP because both had Blackjack."
        printfn "Game result: Draw."
    | _ when newComputerHp <= 0 ->
        printfn ""
        printfn "Computer reached 0 HP first."
        printfn "Game result: You win!"
    | _ when newPlayerHp <= 0 ->
        printfn ""
        printfn "You reached 0 HP first."
        printfn "Game result: You lose."
    | _ ->
        gameLoop newPlayerHp newComputerHp (roundNumber + 1)

[<EntryPoint>]
let main argv =
    printfn "CLI Blackjack Duel"
    printfn "A simplified Blackjack battle game without betting."
    printfn ""
    printfn "Both the player and the computer start with 100 HP."
    printfn "The game continues until one side reaches 0 HP."
    printfn ""

    gameLoop 100 100 1
    0