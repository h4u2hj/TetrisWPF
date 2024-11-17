﻿using TetrisWPFModel.Persistence;

namespace TetrisWPFModel;

public class LBlock : Block
{
    private readonly Position[][] _tiles = new Position[][]
    {
        new Position[] { new(0, 2), new(1, 0), new(1,1), new(1, 2)},
        new Position[] {new(0,1), new(1,1), new(2,1), new(2,2)},
        new Position[] {new(1,0), new(1,1), new(1,2), new(2,0)},
        new Position[] {new(0,0), new(0,1), new(1,1), new(2,1)}
    };

    public LBlock(GameGrid grid) : base(grid) { }

    protected override Position[][] Tiles => _tiles;

    protected override Position StartOffset => new Position(0, 3);
    protected override Position SmallStartOffset => new(0, 0);

    public override int Id => 3;

}