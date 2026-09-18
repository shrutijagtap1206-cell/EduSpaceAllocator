def calculate_capacity_utilization(
    student_capacity: int,
    space_capacity: int
) -> float:

    if space_capacity <= 0:
        return 0.0

    utilization = student_capacity / space_capacity

    return round(
        min(max(utilization, 0.0), 1.0),
        4
    )


def calculate_cost_efficiency(
    rental_cost: float,
    budget: float
) -> float:

    if budget <= 0 or rental_cost <= 0:
        return 0.0

    if rental_cost > budget:
        return 0.0

    efficiency = 0.5 + (
        0.5 * (1.0 - rental_cost / budget)
    )

    return round(
        min(max(efficiency, 0.0), 1.0),
        4
    )


def calculate_student_reach(
    student_capacity: int,
    community_student_count: int
) -> float:

    if community_student_count <= 0:
        return 0.0

    reach = student_capacity / community_student_count

    return round(
        min(max(reach, 0.0), 1.0),
        4
    )


def enrich_matches(
    matches: list[dict],
    spaces: list[dict],
    requests: list[dict],
    communities: list[dict]
) -> list[dict]:

    space_lookup = {
        space["spaceId"]: space
        for space in spaces
    }

    request_lookup = {
        request["requestId"]: request
        for request in requests
    }

    community_lookup = {
        community["communityId"]: community
        for community in communities
    }

    enriched = []

    for match in matches:

        space = space_lookup[match["spaceId"]]
        request = request_lookup[match["requestId"]]

        community = community_lookup.get(
            request["communityId"]
        )

        community_student_count = (
            community["studentCount"]
            if community
            else 0
        )

        capacity_utilization = calculate_capacity_utilization(
            request["studentCapacity"],
            space["capacity"]
        )

        cost_efficiency = calculate_cost_efficiency(
            float(space["rentalCost"]),
            float(request["budget"])
        )

        student_reach = calculate_student_reach(
            request["studentCapacity"],
            community_student_count
        )

        optimization_score = round(
            (
                0.35 * match["score"]
                + 0.25 * capacity_utilization
                + 0.20 * cost_efficiency
                + 0.20 * student_reach
            ),
            4
        )

        enriched.append({
            "matchId": f"EDU-{space['spaceId']}-{request['requestId']}",
            "spaceId": space["spaceId"],
            "requestId": request["requestId"],
            "distanceKm": match["distanceKm"],
            "matchScore": match["score"],
            "capacityUtilization": capacity_utilization,
            "costEfficiency": cost_efficiency,
            "studentReach": student_reach,
            "studentsServed": request["studentCapacity"],
            "optimizationScore": optimization_score
        })

    return enriched


def calculate_summary(matches: list[dict]) -> dict:

    if not matches:
        return {
            "totalStudentsServed": 0,
            "averageDistanceKm": 0.0,
            "averageCapacityUtilization": 0.0,
            "averageCostEfficiency": 0.0,
            "averageStudentReach": 0.0,
            "overallOptimizationScore": 0.0
        }

    count = len(matches)

    return {
        "totalStudentsServed": sum(
            match["studentsServed"]
            for match in matches
        ),
        "averageDistanceKm": round(
            sum(match["distanceKm"] for match in matches) / count,
            2
        ),
        "averageCapacityUtilization": round(
            sum(match["capacityUtilization"] for match in matches) / count,
            4
        ),
        "averageCostEfficiency": round(
            sum(match["costEfficiency"] for match in matches) / count,
            4
        ),
        "averageStudentReach": round(
            sum(match["studentReach"] for match in matches) / count,
            4
        ),
        "overallOptimizationScore": round(
            sum(match["optimizationScore"] for match in matches) / count,
            4
        )
    }
