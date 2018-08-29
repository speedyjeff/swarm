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

