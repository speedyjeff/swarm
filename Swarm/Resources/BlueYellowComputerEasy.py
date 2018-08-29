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

