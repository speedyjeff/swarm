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
