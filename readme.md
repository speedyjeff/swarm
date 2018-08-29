# Swarm

**"A single ant or bee isn't smart, but their colonies are. The study
of swarm intelligence is providing insights that can help humans
manage complex systems, from truck routing to military robots."
- by Peter Miller (published: July 2007)**

[The Genius of Swarms](https://nphsworld16huggins.files.wordpress.com/2015/11/nphsemergencysubplans-1116.pdf)

The game consists of 4 swarms (Yellow, Red, Green and Blue). Each member of the swarm uses the same algorithm for decision making. The individuatal is presented with a limited view of their surroundings, thus forcing decisions based on neighboring individuals and their environment, and not the big picture. There are no inherit leaders, unless one is made. The swarms and individuals take turns making moves.

The objective is to cover (leave squares colored with your color) the majority of the board.

The individuals make decisions based on python scripts. Below are a few examples of the scripts used as the default implementation.
Use the "User" radio button to define your own implementation. 

##Game play
![start screen](https://github.com/speedyjeff/swarm/blob/master/media/start.png)

![hills](https://github.com/speedyjeff/swarm/blob/master/media/hills.png)

![maze](https://github.com/speedyjeff/swarm/blob/master/media/maze.png)

![quad](https://github.com/speedyjeff/swarm/blob/master/media/quad.png)

##Easy
```python
# Parameters:
# p     - Previous state
# h     - Height
# w     - Width
# field - 2 dimensional view of field (from your POV)
#             up
#      [0,0]  ...  [0,n]
# left  ...  [h,w]  ...  right
#      [n,0]  ...  [n,n]
#            down

def Move(p,h,w,field):
  # States:
  # Forbidden   = 0
  # Unoccupied  = 1
  # Occupied    = 2
  # Defended    = 3
  # Duplication = 4
  # Visited     = 5
  # Enemy       = 6
  #
  # Moves:
  # Nothing   = 0
  # Up        = 1
  # Down      = 2
  # Left      = 3
  # Right     = 4
  # Defend    = 5
  # Duplicate = 6

  state = [0,0,0,0,0,0,0]

  up    = field[h-1,w]
  down  = field[h+1,w]
  left  = field[h,w-1]
  right = field[h,w+1]

  state[ up ]    += 1
  state[ down ]  += 1
  state[ left ]  += 1
  state[ right ] += 1

  # make a move
  if 2 <= state[0]: # PlotState.Forbidden
    # if in a corner
    return 6 # Move.Duplicate
  if up == 1 or up == 6: # PlotState.Unoccupied or PlotState.Enemy
    return 1 # Move.Up
  if right == 5 or right == 1 or right == 6: # PlotState.Visited or PlotState.Unoccupied or PlotState.Enemy
    return 4 # Move.Right
  if left == 1 or left == 6: # PlotState.Unoccupied or PlotState.Enemy
    return 3 # Move.Left
  if down == 5 or down == 1 or down == 6: # PlotState.Visited or PlotState.Unoccupied or PlotState.Enemy
    return 2 # Move.Down
  else:
    if right == 1 or right == 6 or right == 5: # PlotState.Unoccupied or PlotState.Enemy or PlotState.Visited
      return 4 # Move.Right
    if down == 1 or down == 6 or down == 5: # PlotState.Unoccupied or PlotState.Enemy or PlotState.Visited
      return 2 # Move.Down
    if left == 1 or left == 6 or left == 5: # PlotState.Unoccupied or PlotState.Enemy or PlotState.Visited
      return 3 # Move.Left
    if up == 1 or up == 6 or up == 5: # PlotState.Unoccupied or PlotState.Enemy or PlotState.Visited
      return 1 # Move.Up

  # default move
  return 5 # Move.Defend
```

##Hard
```python
# Parameters:
# p     - Previous state
# h     - Height
# w     - Width
# field - 2 dimensional view of field (from your POV)
#             up
#      [0,0]  ...  [0,n]
# left  ...  [h,w]  ...  right
#      [n,0]  ...  [n,n]
#            down

def Move(p,h,w,field):
  # States:
  # Forbidden   = 0
  # Unoccupied  = 1
  # Occupied    = 2
  # Defended    = 3
  # Duplication = 4
  # Visited     = 5
  # Enemy       = 6
  #
  # Moves:
  # Nothing   = 0
  # Up        = 1
  # Down      = 2
  # Left      = 3
  # Right     = 4
  # Defend    = 5
  # Duplicate = 6

  state = [0,0,0,0,0,0,0]

  up    = field[h-1,w]
  down  = field[h+1,w]
  left  = field[h,w-1]
  right = field[h,w+1]

  state[ up ]    += 1
  state[ down ]  += 1
  state[ left ]  += 1
  state[ right ] += 1

  # make a move
  if 2 <= state[0]:
    # if in a corner
    return 6 # Move.Duplicate
  if 0 < state[6] and p != 3: # PlotState.Defend
    return 5 # Move.Defend
  if 0 < state[6]: # PlotState.Enemy
    # find the PlotState.Enemy and stomp them
    if up == 6: # PlotState.Enemy
      return 1 # Move.Up
    if down == 6: # PlotState.Enemy
      return 2 # Move.Down
    if left == 6: # PlotState.Enemy
      return 3 # Move.Left
    if right == 6: # PlotState.Enemy
      return 4 # Move.Right
  if 0 < state[1]: # PlotState.Unoccupied
    if left == 1: # PlotState.Unoccupied
      return 3 # Move.Left
    if down == 1: # PlotState.Unoccupied
      return 2 # Move.Down
    if right == 1: # PlotState.Unoccupied
      return 4 # Move.Right
    if up == 1: # PlotState.Unoccupied
      return 1 # Move.Up
  else:
    if left == 5: # PlotState.Visited
      return 3 # Move.Left
    if down == 5: # PlotState.Visited
      return 2 # Move.Down
    if right == 5: # PlotState.Visited
      return 4 # Move.Right
    if up == 5: # PlotState.Visited
      return 1 # Move.Up

  # default move
  return 5 # Move.Defend
```

##Random
```python
# Parameters:
# p     - Previous state
# h     - Height
# w     - Width
# field - 2 dimensional view of field (from your POV)
#             up
#      [0,0]  ...  [0,n]
# left  ...  [h,w]  ...  right
#      [n,0]  ...  [n,n]
#            down

import _random
rand = _random.Random()

def Rand():
  return rand.getrandbits(8)

def Avaiable(d):
  if d == 1 or d == 5 or d == 6: # PlotState.Unoccupied or PlotState.Visisted or PlotState.Enemy
    return 1
  else:
    return 0

def Move(p,h,w,field):
  # States:
  # Forbidden   = 0
  # Unoccupied  = 1
  # Occupied    = 2
  # Defended    = 3
  # Duplication = 4
  # Visited     = 5
  # Enemy       = 6
  #
  # Moves:
  # Nothing   = 0
  # Up        = 1
  # Down      = 2
  # Left      = 3
  # Right     = 4
  # Defend    = 5
  # Duplicate = 6

  up    = field[h-1,w]
  down  = field[h+1,w]
  left  = field[h,w-1]
  right = field[h,w+1]

  while 1 == 1: # forever
    move = (Rand() % 6) + 1
    if 1 <= move and move <= 4:
      if move == 1 and Avaiable(up): # Move.Up
        return 1 # Move.Up
      elif move == 2 and Avaiable(down): # Move.Down
        return 2 # Move.Down
      elif move == 3 and Avaiable(left): # Move.Left
        return 3 # Move.Left
      elif move == 4 and Avaiable(right): # Move.Right
        return 4 # Move.Right
    else:
        return move
```

##Human
```python
# Parameters:
# p     - Previous state
# h     - Height
# w     - Width
# field - 2 dimensional view of field (from your POV)
#             up
#      [0,0]  ...  [0,n]
# left  ...  [h,w]  ...  right
#      [n,0]  ...  [n,n]
#            down

def AdjacentMove(up,down,left,right):
  if up == 1 or up == 6: # PlotState.Unoccupied or PlotState.Enemy
    return 1 # Move.Up
  elif right == 1 or right == 6: # PlotState.Unoccupied or PlotState.Enemy
    return 4 # Move.Right
  elif down == 1 or down == 6: # PlotState.Unoccupied or PlotState.Enemy
    return 2 # Move.Down
  elif left == 1 or left == 6: # PlotState.Unoccupied or PlotState.Enemy
    return 3 # Move.Left
  elif down == 5: # PlotState.Visited
    return 2 # Move.Down
  elif left == 5: # PlotState.Visited
    return 3 # Move.left
  elif right == 5: # PlotState.Visited
    return 4 # Move.Right
  elif up == 5: # PlotState.Visited
    return 1 # Move.Up
  else:
    return 0 # Move.Nothing

def Avaiable(d):
  if d == 1 or d == 5 or d == 6: # PlotState.Unoccupied or PlotState.Visisted or PlotState.Enemy
    return 1
  else:
    return 0

def Move(p,h,w,field):
  # States:
  # Forbidden   = 0
  # Unoccupied  = 1
  # Occupied    = 2
  # Defended    = 3
  # Duplication = 4
  # Visited     = 5
  # Enemy       = 6
  #
  # Moves:
  # Nothing   = 0
  # Up        = 1
  # Down      = 2
  # Left      = 3
  # Right     = 4
  # Defend    = 5
  # Duplicate = 6

  state  = [0,0,0,0,0,0,0]
  nstate = [0,0,0,0,0,0,0]

  up    = field[h-1,w]
  down  = field[h+1,w]
  left  = field[h,w-1]
  right = field[h,w+1]

  nup    = field[h-2,w]
  ndown  = field[h+2,w]
  nleft  = field[h,w-2]
  nright = field[h,w+2]

  state[ up ]    += 1
  state[ down ]  += 1
  state[ left ]  += 1
  state[ right ] += 1

  nstate[ nup ]    += 1
  nstate[ ndown ]  += 1
  nstate[ nleft ]  += 1
  nstate[ nright ] += 1

  # make a move
  if 2 <= state[0]: # PlotState.Forbidden
    # if in a corner
    return 6 # Move.Duplicate
  elif (nup == 0 and nleft == 0 and up != 0 and left != 0) or (nup == 0 and nright == 0 and up != 0 and right != 0) or (ndown == 0 and nleft == 0 and down != 0 and left != 0) or (ndown == 0 and nright == 0  and down != 0 and right != 0): # PlotState.Forbidden - in corner stone
    return 5 # Move.Defend
  elif 1 <= state[4]: # PlotState.Duplication
    # grab the corner stone
    if (up == 0 and Avaiable(down) == 1): # PlotState.Forbidden - Move to the corner stone
      return 2 # Move.Down
    elif (down == 0 and Avaiable(up) == 1): # PlotState.Forbidden - Move to the corner stone
      return 1 # Move.Up
    elif (right == 0 and Avaiable(left) == 1): # PlotState.Forbidden - Move to the corner stone
      return 3 # Move.Left
    elif (left == 0 and Avaiable(right) == 1): # PlotState.Forbidden - Move to the corner stone
      return 4 # Move.Right
    else:
      return AdjacentMove(up,down,left,right)
  elif 1 <= nstate[4]: # PlotState.Duplication
    # protect the duplicater
    if (nup == 4 and up == 2 and down != 6 and ndown != 6) or (ndown == 4 and down == 2 and up != 6 and nup !=6) or (nleft == 4 and left == 2 and left != 6 and nleft != 6) or (nright == 4 and right == 2 and left != 6 and nright != 6): # PlotState.Duplication and PlotState.Occupied and !PlotState.Enemy - Make way for the new member of the swarm
      return AdjacentMove(up,down,left,right)
    else:
      return 5 # Move.Defend
  else:
      return AdjacentMove(up,down,left,right)

  # default move
  return 5 # Move.Defend

```