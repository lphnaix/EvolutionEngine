from dataclasses import dataclass


@dataclass
class ItemConfig:
    id: str
    name: str
    description: str | None = None


@dataclass
class EngineSettings:
    player_speed: float = 5.0
    world_width: int = 32
    world_height: int = 32
    world_seed: int = 1337
