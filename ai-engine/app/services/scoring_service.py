import math

MAX_RADIUS_KM = 10.0


def haversine_distance(
    lat1: float,
    lon1: float,
    lat2: float,
    lon2: float
) -> float:
    earth_radius_km = 6371.0

    d_lat = math.radians(lat2 - lat1)
    d_lon = math.radians(lon2 - lon1)

    a = (
        math.sin(d_lat / 2) ** 2
        + math.cos(math.radians(lat1))
        * math.cos(math.radians(lat2))
        * math.sin(d_lon / 2) ** 2
    )

    c = 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a))

    return earth_radius_km * c


def estimated_cost(space: dict) -> float:
    return float(space["rentalCost"])


def calculate_score(
    space: dict,
    request: dict,
    distance_km: float
) -> float:

    capacity_score = min(
        space["capacity"] / request["studentCapacity"],
        1.0
    )

    distance_score = max(
        0.0,
        1.0 - distance_km / MAX_RADIUS_KM
    )

    cost = estimated_cost(space)

    if cost <= 0:
        budget_score = 0.0
    else:
        budget_score = min(
            request["budget"] / cost,
            1.0
        )

    availability_score = (
        1.0 if space["availability"] else 0.0
    )

    score = (
        0.30 * capacity_score
        + 0.30 * distance_score
        + 0.25 * budget_score
        + 0.15 * availability_score
    )

    return round(score, 4)


def is_feasible(
    space: dict,
    request: dict
) -> tuple[bool, float, str]:

    if not space["availability"]:
        return False, 0.0, "Space unavailable"

    if space["capacity"] < request["studentCapacity"]:
        return False, 0.0, "Insufficient capacity"

    if space["rentalCost"] > request["budget"]:
        return False, 0.0, "Over budget"

    distance_km = haversine_distance(
        space["latitude"],
        space["longitude"],
        request["preferredLatitude"],
        request["preferredLongitude"]
    )

    if distance_km > MAX_RADIUS_KM:
        return False, distance_km, "Outside maximum radius"

    score = calculate_score(
        space,
        request,
        distance_km
    )

    return True, distance_km, "Feasible"


def generate_candidates(
    spaces: list[dict],
    requests: list[dict]
) -> list[dict]:

    candidates = []

    for space in spaces:
        for request in requests:

            feasible, distance_km, reason = is_feasible(
                space,
                request
            )

            if not feasible:
                continue

            score = calculate_score(
                space,
                request,
                distance_km
            )

            candidates.append({
                "spaceId": space["spaceId"],
                "requestId": request["requestId"],
                "distanceKm": round(distance_km, 2),
                "score": score,
                "reason": reason
            })

    return candidates
