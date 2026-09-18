import networkx as nx

from app.services.scoring_service import generate_candidates
from app.services.optimization_service import (
    enrich_matches,
    calculate_summary
)


def build_bipartite_graph(
    spaces: list[dict],
    requests: list[dict],
    candidates: list[dict]
):
    graph = nx.Graph()

    space_nodes = [
        f"SP-{space['spaceId']}"
        for space in spaces
    ]

    request_nodes = [
        f"REQ-{request['requestId']}"
        for request in requests
    ]

    graph.add_nodes_from(
        space_nodes,
        bipartite=0
    )

    graph.add_nodes_from(
        request_nodes,
        bipartite=1
    )

    for candidate in candidates:
        graph.add_edge(
            f"SP-{candidate['spaceId']}",
            f"REQ-{candidate['requestId']}",
            weight=candidate["score"],
            distanceKm=candidate["distanceKm"]
        )

    return graph


def run_matching(
    spaces: list[dict],
    requests: list[dict],
    communities: list[dict]
):

    candidates = generate_candidates(
        spaces,
        requests
    )

    graph = build_bipartite_graph(
        spaces,
        requests,
        candidates
    )

    matching = nx.algorithms.matching.max_weight_matching(
        graph,
        maxcardinality=True,
        weight="weight"
    )

    candidate_lookup = {
        (
            candidate["spaceId"],
            candidate["requestId"]
        ): candidate
        for candidate in candidates
    }

    raw_matches = []

    for node_a, node_b in matching:

        if node_a.startswith("SP-"):
            space_node = node_a
            request_node = node_b
        else:
            space_node = node_b
            request_node = node_a

        space_id = int(
            space_node.replace("SP-", "")
        )

        request_id = int(
            request_node.replace("REQ-", "")
        )

        candidate = candidate_lookup.get(
            (space_id, request_id)
        )

        if candidate:
            raw_matches.append(candidate)

    matches = enrich_matches(
        raw_matches,
        spaces,
        requests,
        communities
    )

    return {
        "candidateCount": len(candidates),
        "matchCount": len(matches),
        "matches": matches,
        "summary": calculate_summary(matches)
    }
