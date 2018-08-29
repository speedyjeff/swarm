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

