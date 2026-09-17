from fastapi import APIRouter, HTTPException
from app.services.clustering_service import run_kmeans

router = APIRouter()


@router.post("/cluster")
def cluster_communities(payload: dict):
    communities = payload.get("communities", [])
    n_clusters = payload.get("n_clusters", 3)

    try:
        results = run_kmeans(
            communities,
            n_clusters
        )

        return {
            "clusterCount": n_clusters,
            "results": results
        }

    except ValueError as exc:
        raise HTTPException(
            status_code=400,
            detail=str(exc)
        )
