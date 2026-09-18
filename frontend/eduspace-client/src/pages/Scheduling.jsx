import { useEffect, useState } from "react";
import api from "../services/api";

export default function Scheduling() {
  const [items, setItems] = useState([]);
  useEffect(() => { api.get("/Scheduling").then(r => setItems(r.data)); }, []);
  return <div className="page"><h1>Scheduling</h1><p>Planned sessions linked to allocated learning spaces.</p>
    <div className="card"><table><thead><tr><th>ID</th><th>Course</th><th>Allocation</th><th>Date</th><th>Time</th><th>Status</th></tr></thead>
    <tbody>{items.map(x => <tr key={x.scheduleId}><td>{x.scheduleId}</td><td>{x.course?.courseName ?? x.courseId}</td><td>{x.allocationId}</td><td>{String(x.sessionDate).slice(0,10)}</td><td>{x.startTime} - {x.endTime}</td><td>{x.status}</td></tr>)}</tbody></table></div>
  </div>;
}
