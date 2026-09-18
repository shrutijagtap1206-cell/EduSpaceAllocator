import { useEffect, useState } from "react";
import api from "../services/api";

export default function AuditLogs() {
  const [items, setItems] = useState([]);
  useEffect(() => { api.get("/Audit").then(r => setItems(r.data)); }, []);
  return <div className="page"><h1>Audit Logs</h1><p>Traceable records of system actions.</p>
    <div className="card"><table><thead><tr><th>Time</th><th>Action</th><th>Entity</th><th>Details</th></tr></thead>
    <tbody>{items.map(x => <tr key={x.auditLogId}><td>{new Date(x.createdAt).toLocaleString()}</td><td>{x.action}</td><td>{x.entityType} {x.entityId}</td><td>{x.details}</td></tr>)}</tbody></table></div>
  </div>;
}
