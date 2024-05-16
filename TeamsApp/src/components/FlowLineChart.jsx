import React from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

const FlowLineChart = ({ flows }) => {
    if (!flows || flows.length === 0) {
        return <div>No data available</div>;
    }

    const lineColors = [
        "#8884d8",
        "#82ca9d",
        "#ffc658",
        "#ff7300",
        "#d0ed57",
        "#a4de6c",
    ];

    const statusMap = {
        "Failed": 0,
        "Succeeded": 1,
    };

    const getStatusValue = (status) => {
        if (status in statusMap) {
            return statusMap[status];
        } else {
            return 0; // Default to 0 if status is not found in statusMap
        }
    };

    // Convert status values to numbers
    const transformedData = flows.map(flow => ({
        name: flow.name,
        data: flow.data.map(entry => ({
            ...entry,
            status: getStatusValue(entry.status)
        }))
    }));

    return (
        <ResponsiveContainer width="100%" height="100%">
            <LineChart>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="time" type="category" allowDuplicatedCategory={false} />
                <YAxis
                    domain={[-0.1, 1.1]}
                    ticks={[0, 1]}
                    tickFormatter={(value) => value === 0 ? "Failed" : "Succeeded"} // Custom tick formatter
                />
                <Tooltip />
                <Legend />
                {transformedData.map((s, index) => (
                    <Line
                        key={s.name}
                        type="monotone"
                        dataKey="status"
                        data={s.data || []}
                        name={s.name}
                        stroke={lineColors[index % lineColors.length]}
                    />
                ))}
            </LineChart>
        </ResponsiveContainer>
    );
}

export default FlowLineChart;