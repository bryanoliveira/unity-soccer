# Unity Soccer

A [Soccer-Twos](https://github.com/Unity-Technologies/ml-agents/blob/92ff2c26fef7174b443115454fa1c6045d622bc2/docs/Learning-Environment-Examples.md#soccer-twos) game variant. Built to work as a reinforcement learning environment with [this package](https://github.com/bryanoliveira/soccer-twos-env/).

<div align="center">
    <img class="text-img mw-100" src="https://raw.githubusercontent.com/bryanoliveira/unity-soccer/main/images/soccer.gif">
</div>
<br/>

Features:

- Added a "watching" mode with better visualization and cool animations;
- Added position & velocity setting communication channels (for e.g. curriculum learning);
- Simplified integration with Python

## Environment Specs

This environment is based on Unity ML Agents' [Soccer Twos](https://github.com/Unity-Technologies/ml-agents/blob/92ff2c26fef7174b443115454fa1c6045d622bc2/docs/Learning-Environment-Examples.md#soccer-twos), so most of the specs are the same. Here, four agents compete in a 2 vs 2 toy soccer game, aiming to get the ball into the opponent's goal while preventing the ball from entering own goal.

<div align="center">
    <img src="https://raw.githubusercontent.com/bryanoliveira/unity-soccer/main/images/obs.png" width="480"/>
</div>
<br/>

- Observation space: a 336-dimensional vector corresponding to 11 ray-casts forward distributed over 120 degrees and 3 ray-casts backward distributed over 90 degrees, each detecting 6 possible object types, along with the object's distance. The forward ray-casts contribute 264 state dimensions and backward 72 state dimensions over three observation stacks.
- Action space: 3 discrete branched actions (MultiDiscrete) corresponding to forward, backward, sideways movement, as well as rotation (27 discrete actions).
- Agent Reward Function:
  - `1 - accumulated time penalty`: when ball enters opponent's goal. Accumulated time penalty is incremented by `(1 / MaxSteps)` every fixed update and is reset to 0 at the beginning of an episode. In this build, `MaxSteps = 5000`.
  - `-1`: when ball enters team's goal.