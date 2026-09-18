from fastapi import APIRouter

from app.services.matching_service import run_matching

router = APIRouter()


@router.post("/optimization")
def optimization(payload: dict):

    spaces = payload.get("spaces", [])
    requests = payload.get("requests", [])
    communities = payload.get("communities", [])

    result = run_matching(
        spaces,
        requests,
        communities
    )

    return result
