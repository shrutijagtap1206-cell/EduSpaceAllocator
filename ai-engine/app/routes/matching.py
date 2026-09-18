from fastapi import APIRouter, HTTPException

from app.services.matching_service import run_matching

router = APIRouter()


@router.post("/matching")
def matching(payload: dict):

    spaces = payload.get("spaces", [])
    requests = payload.get("requests", [])
    communities = payload.get("communities", [])

    if not spaces:
        raise HTTPException(
            status_code=400,
            detail="At least one space is required."
        )

    if not requests:
        raise HTTPException(
            status_code=400,
            detail="At least one learning request is required."
        )

    try:
        return run_matching(
            spaces,
            requests,
            communities
        )

    except (KeyError, TypeError, ValueError) as exc:
        raise HTTPException(
            status_code=400,
            detail=str(exc)
        )
