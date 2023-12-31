﻿namespace life;

internal class Entity
{
    public string Character = "0";
    public ConsoleColor EntityColour;

    public int PositionX;
    public int PositionY;
    public int IterationsUntilDeath = 48;
    public bool FoodPatternGene;

    public List<Food> CollectedFood = new();

    int upValue = 1;
    int downValue = 1;
    int leftValue = 1;
    int rightValue = 1;

    public Entity(int positionX, int positionY, bool foodPatternGene, ConsoleColor entityColour)
    {
        PositionX = positionX;
        PositionY = positionY;
        FoodPatternGene = foodPatternGene;
        EntityColour = entityColour;
    }

    public void Move()
    {
        int previousX = PositionX;
        int previousY = PositionY;

        Random random = new();
        int direction = random.Next(1, 5);

        int newPosX = PositionX;
        int newPosY = PositionY;

        switch (direction)
        {
            case 1:
                newPosY = Math.Min(Program.GRID_HEIGHT - 1, PositionY + 1); // down
                break;

            case 2:
                newPosY = Math.Max(0, PositionY - 1); // up
                break;

            case 3:
                newPosX = Math.Min(Program.GRID_WIDTH - 1, PositionX + 1); // right
                break;

            case 4:
                newPosX = Math.Max(0, PositionX - 1); // left
                break;
        }

        if (!EntityOrFoodClusterAtPosition(newPosX, newPosY))
        {
            Console.SetCursorPosition(previousX, previousY);
            Console.Write(Program.NODE);

            if (FoodAtPosition(newPosX, newPosY))
            {
                Console.SetCursorPosition(newPosX, newPosY);
            }

            Console.ForegroundColor = EntityColour;
            Console.SetCursorPosition(newPosX, newPosY);
            Console.Write(Character);
            Console.ResetColor();

            PositionX = newPosX;
            PositionY = newPosY;
        }
    }


    private bool FoodAtPosition(int positionX, int positionY)
    {
        Food foodToRemove = null;

        foreach (Food food in Program.ListOfFood)
        {
            if (food.PositionX == positionX && food.PositionY == positionY)
            {
                foodToRemove = food;
                break;
            }
        }

        if (foodToRemove != null)
        {
            Program.ListOfFood.Remove(foodToRemove);
            CollectedFood.Add(foodToRemove);
            return true;
        }
        return false;
    }


    public static bool EntityOrFoodClusterAtPosition(int positionX, int positionY) 
    {
        foreach (Entity entity in Program.ListOfEntities)
        {
            if (entity.PositionX == positionX && entity.PositionY == positionY)
            {
                return true;
            }
        }
        foreach (FoodCluster foodCluster in Program.ListOfFoodClusters)
        {
            if (foodCluster.PositionX == positionX && foodCluster.PositionY == positionY)
            {
                return true;
            }
        }
        return false;
    }

    public void ConsumeFood() 
    {
        if (CollectedFood.Count > 0) 
        {
            CollectedFood.RemoveAt(0);
            IterationsUntilDeath = 20;
        }
    }
    public void Reproduce()
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newPositionX = PositionX + i;
                int newPositionY = PositionY + j;

                if (newPositionX >= 0 && newPositionX < Program.GRID_WIDTH &&
                    newPositionY >= 0 && newPositionY < Program.GRID_HEIGHT &&
                    !EntityOrFoodClusterAtPosition(newPositionX, newPositionY))
                {
                    Entity newEntity = new(newPositionX, newPositionY, FoodPatternGene, EntityColour);
                    Program.ListOfEntities.Add(newEntity);

                    Console.SetCursorPosition(newPositionX, newPositionY);
                    Console.ForegroundColor = newEntity.EntityColour;
                    Console.Write(newEntity.Character);
                    Console.ResetColor();

                    CollectedFood.RemoveAt(0);

                    return;
                }
            }
        }
    }

    public void FoodPatternMove()
    {
        double decayFactor = 0.9;

        upValue = (int)Math.Max(1, upValue * decayFactor);
        downValue = (int)Math.Max(1, downValue * decayFactor);
        leftValue = (int)Math.Max(1, leftValue * decayFactor);
        rightValue = (int)Math.Max(1, rightValue * decayFactor);

        double totalSum = upValue + downValue + leftValue + rightValue;

        double upValuePercentage = (100 / totalSum) * upValue;
        double downValuePercentage = (100 / totalSum) * downValue;
        double leftValuePercentage = (100 / totalSum) * leftValue;
        double rightValuePercentage = (100 / totalSum) * rightValue;

        double randomNumber = new Random().NextDouble() * 100;

        if (randomNumber < upValuePercentage) MoveEntity(0, -1, ref upValue);
        else if (randomNumber < upValuePercentage + downValuePercentage) MoveEntity(0, 1, ref downValue);
        else if (randomNumber < upValuePercentage + downValuePercentage + leftValuePercentage) MoveEntity(-1, 0, ref leftValue);
        else MoveEntity(1, 0, ref rightValue);
    }

    private void MoveEntity(int deltaX, int deltaY, ref int directionValue)
    {
        int newPosX = Math.Max(0, Math.Min(PositionX + deltaX, Program.GRID_WIDTH - 1));
        int newPosY = Math.Max(0, Math.Min(PositionY + deltaY, Program.GRID_HEIGHT - 1));

        if (!EntityOrFoodClusterAtPosition(newPosX, newPosY))
        {
            Console.SetCursorPosition(PositionX, PositionY);
            Console.Write(Program.NODE);

            if (FoodAtPosition(newPosX, newPosY))
            {
                Console.SetCursorPosition(newPosX, newPosY);
                directionValue++;
            }

            Console.ForegroundColor = EntityColour;
            Console.SetCursorPosition(newPosX, newPosY);
            Console.Write(Character);
            Console.ResetColor();

            PositionX = newPosX;
            PositionY = newPosY;
        }
    }

}

